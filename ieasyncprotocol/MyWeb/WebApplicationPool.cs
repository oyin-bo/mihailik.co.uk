using System;

using System.Collections;
using System.IO;
using System.Web.Hosting;

namespace Mihailik.InternetExplorer.Protocols
{
	public sealed class WebApplicationPool
	{
        private WebApplicationPool(){}

        static readonly Hashtable hosts=new Hashtable();

        public static LocalWebHost GetHost(System.Uri url)
        {
            if( url==null )
                throw new ArgumentNullException("childPath");

            System.Uri found=url;
            while( true )
            {
                if( File.Exists(found.LocalPath) )
                {
                    return GetHostByDirectory( Path.GetDirectoryName(found.LocalPath) );
                }
                else if( Directory.Exists(found.LocalPath) )
                {
                    return GetHostByDirectory(found.LocalPath);
                }
                else
                {
                    found=GetParentUri(found);
                    if( found==null )
                        throw new DirectoryNotFoundException();
                }
            }
        }

        static LocalWebHost GetHostByDirectory(string directory)
        {
            if( directory==null )
                throw new ArgumentNullException("directory");

            string relative=null;
            string root=directory;
            if( root.EndsWith("\\") )
                root=root.Substring(0,root.Length-1);

            while( true )
            {
                if( File.Exists(Path.Combine(root,"global.asax")) )
                    return GetHostByRootDirectory(root);
                
                string parent=Path.GetDirectoryName(root);

                if( parent==null )
                    return GetHostByRootDirectory(root);

                if( relative!=null )
                    relative="/"+relative;
                relative=Path.GetFileName(root)+relative;

                root=parent;
            }
        }

        static LocalWebHost GetHostByRootDirectory(string root)
        {
            if( root==null )
                throw new ArgumentNullException("root");

            root=root.ToLower();

            lock( hosts )
            {
                LocalWebHost result=hosts[root] as LocalWebHost;

                if( result!=null )
                    try
                    {
                        result.Kick();
                    }
                    catch
                    {
                        result=null;
                    }

                if( result==null )
                {
                    result=CreateHost(root);
                    hosts[root]=result;
                }

                return result;
            }            
        }

        static LocalWebHost CreateHost(string appRoot)
        {
            appRoot=appRoot.ToLower();

            if( appRoot==null )
                throw new ArgumentNullException("appRoot");

            string appVeryRoot=Path.GetPathRoot(appRoot);

            string virtualAppRoot=
                "/"+appRoot.Substring(appVeryRoot.Length);
            virtualAppRoot=virtualAppRoot.Replace("\\","/");

            object resultObj=ApplicationHost.CreateApplicationHost(
                typeof(LocalWebHost),
                virtualAppRoot,
                appRoot );

            if( resultObj==null )
#if DEBUG
                throw new InvalidOperationException("Return value of CreateApplicationHost is null. Virtual root: "+virtualAppRoot+", app root: "+appRoot+".");
#else
                throw new InvalidOperationException("Return value of CreateApplicationHost is null.");
#endif

            LocalWebHost result=resultObj as LocalWebHost;

            if( result==null )
#if DEBUG
                throw new InvalidOperationException("Return value of CreateApplicationHost of wrong type ("+resultObj.GetType()+"). Virtual root: "+virtualAppRoot+", app root: "+appRoot+".");
#else
                throw new InvalidOperationException("Return value of CreateApplicationHost of wrong type ("+resultObj.GetType()+").");
#endif
            result.Init(virtualAppRoot,appRoot);


            return result;
        }

        public static System.Uri GetParentUri(System.Uri childPath)
        {
            if( childPath==null )
                throw new ArgumentNullException("childPath");

            string parentDir=Path.GetDirectoryName( childPath.LocalPath );
            if( parentDir==null )
                return null;

            if( childPath.IsDefaultPort )
                return new Uri( childPath.Scheme+"://"+parentDir );
            else
                return new Uri( childPath.Scheme+":"+childPath.Port+"//"+parentDir );
        }
	}
}

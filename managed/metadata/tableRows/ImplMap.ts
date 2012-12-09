/// <reference path="../TableStreamReader.ts" />
module pe.managed.metadata {
	//The TabeKind.ImplMap table holds information about unmanaged methods
	//that can be reached from managed code, using PInvoke dispatch.
	//Each row of the TableKind.ImplMap table associates a row in the TableKind.MethodDef table
	//(MemberForwarded)
	//with the name of a routine (ImportName) in some unmanaged DLL (ImportScope).
	//[ECMA-335 para22.22]
	export class ImplMap {
		//A 2-byte bitmask of type PInvokeAttributes, ECMA-335 para23.1.8.
		mappingFlags: PInvokeAttributes;

		//An index into the TableKind.Field or TableKind.MethodDef table;
		//more precisely, a MemberForwarded (ECMA-335 para24.2.6) coded index.
		//However, it only ever indexes the TableKind.MethodDef table, since Field export is not supported.
		memberForwarded: CodedIndex;

		importName: string;

		//An index into the TableKind.ModuleRef table.
		importScope: number;

		read(reader: TableStreamReader): void {
			this.mappingFlags = reader.readShort();
			this.memberForwarded = reader.readMemberForwarded();
			this.importName = reader.readString();
			this.importScope = reader.readTableRowIndex(TableKind.ModuleRef);
		}
	}
}
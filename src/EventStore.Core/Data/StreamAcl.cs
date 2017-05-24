namespace EventStore.Core.Data
{
    public class StreamAcl
    {
        public readonly string[] ReadRoles;
        public readonly string[] WriteRoles;
        public readonly string[] DeleteRoles;
        public readonly string[] MetaReadRoles;
        public readonly string[] MetaWriteRoles;

        public StreamAcl(string readRole, string writeRole, string deleteRole, string metaReadRole, string metaWriteRole)
                : this(readRole == null ? null : new[]{readRole},
                       writeRole == null ? null : new[]{writeRole},
                       deleteRole == null ? null : new[]{deleteRole},
                       metaReadRole == null ? null : new[]{metaReadRole},
                       metaWriteRole == null ? null : new[]{metaWriteRole})
        {
        }

        public StreamAcl(string[] readRoles, string[] writeRoles, string[] deleteRoles, string[] metaReadRoles, string[] metaWriteRoles)
        {
            ReadRoles = readRoles;
            WriteRoles = writeRoles;
            DeleteRoles = deleteRoles;
            MetaReadRoles = metaReadRoles;
            MetaWriteRoles = metaWriteRoles;
        }

        public override string ToString() =>
            $"Read: {(ReadRoles == null ? "<null>" : "[" + string.Join(", ", ReadRoles) + "]")}, "
            + $"Write: {(WriteRoles == null ? "<null>" : "[" + string.Join(",", WriteRoles) + "]")}, "
            + $"Delete: {(DeleteRoles == null ? "<null>" : "[" + string.Join(",", DeleteRoles) + "]")}, "
            + $"MetaRead: {(MetaReadRoles == null ? "<null>" : "[" + string.Join(",", MetaReadRoles) + "]")}, "
            + $"MetaWrite: {(MetaWriteRoles == null ? "<null>" : "[" + string.Join(",", MetaWriteRoles) + "]")}";
    }
}
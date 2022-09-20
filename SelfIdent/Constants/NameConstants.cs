
namespace SelfIdent.Constants;

internal static class NameConstants
{
    public const string TABLE_MAINUSER = "NETIDENT_IDENTITY";
    public const string COL_ID = "ID";
    public const string COL_NAME = "NAME";
    public const string COL_SALT = "SALT";
    public const string COL_EMAIL = "EMAIL";
    public const string COL_HASH = "HASH";

    public const string TABLE_ADDITIONALDATA = "NETIDENT_ADDITIONALDATA";
    public const string COL_ATTEMPTS = "ATTEMPS";
    public const string COL_USERID = "USERID";
    public const string COL_REGISTERDATE = "REGISTERDATE";
    public const string COL_LASTLOGON = "LASTLOGON";
    public const string COL_ITERATIONS = "ITERATIONS";
    public const string COL_PBKDFFUNCTION = "PBKDFFUNCTION";
    public const string COL_PASSWORDBYTELENGTH = "PASSWORDBYTELENGTH";
    public const string COL_SALTBYTELENGTH = "SALTBYTELENGTH";
    public const string COL_LOCKED = "LOCKED";
    public const string COL_LOCKKEY = "LOCKKEY";
    public const string COL_HASHINGFUNCTION = "HASHFUNCTION";
    public const string COL_SCRYPTTHREADS = "SCRYPTTHREADS";
    public const string COL_SCRYPTBLOCKSIZE = "SCRYPTBLOCKSIZE";
    public const string COL_ARGONTHREADS = "ARGONTHREADS";
    public const string COL_ARGONMEMORY = "ARGONMEMORY";
    public const string COL_ARGONTIME = "ARGONTIME";
    public const string COL_CHANGEDKEY = "CHANGEDKEY";
    public const string COL_INTEGRITYSTATUS = "INTEGRITYSTATUS";

    public const string TABLE_MFA = "NETIDENT_MFA";
    public const string COL_SESSIONID = "SESSIONID";
    public const string COL_MFAKEY = "MFAKEY";
    public const string COL_TIMEISSUED = "TIMEISSUED";
    public const string COL_TIMEEXPIRED = "TIMEEXPIRED";

    public const string TABLE_CUSTOMDATA = "NETIDENT_CUSTOMDATA";

    public const string TABLE_ROLES = "NETIDENT_ROLES";
    public const string COL_ROLENUMERICVALUE = "ROLENUMERICVALUE";
    public const string COL_ROLENAME = "ROLENAME";

    public const string TABLE_ROLESUSERS = "NETIDENT_ROLESUSERS";
    public const string COL_ROLEID = "ROLEID";
}

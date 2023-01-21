// The System.DirectoryServices nuget package is only avaiable on Windows.
// If you are building for Linux or MacOS, this won't work.
using System;
using System.DirectoryServices;
//using System.DirectoryServices.AccountManagement;

namespace dotNetCoreADSample
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryEntry objADAM;   // Binding object.

            // Construct the binding string.
            // This is the Domain Controller and OU where you want to create the group.
            string strOUPath = "LDAP://server01:389/ou=TestOU,dc=contoso,dc=com";

            #region Create Group
            DirectoryEntry objGroup;  // Group object.
            string strGroupDescription = "AD LDS Test Group";    // Description of group.
            string strGroupDisplayName = "Test Group";    // Display name of group.
            string strGroupSAMAccountName = "test";  // Group's SAMAccountName
            string strGroup = "cn=TestGroup";          // Group to create.

            Console.WriteLine("Bind to: {0}", strOUPath);

            // Get a reference to the OU where you want to create the group.
            try
            {
                objADAM = new DirectoryEntry(strOUPath);
                objADAM.RefreshCache();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:   Bind failed.");
                Console.WriteLine("         {0}", e.Message);
                return;
            }


            Console.WriteLine("Create:  {0}", strGroup);

            // Create Group.
            try
            {
                objGroup = objADAM.Children.Add(strGroup, "group");
                objGroup.Properties["displayName"].Add(strGroupDisplayName);
                objGroup.Properties["description"].Add(strGroupDescription);
                objGroup.Properties["sAMAccountName"].Add(strGroupSAMAccountName);
                objGroup.CommitChanges();

                //// Output Group attributes.
                //Console.WriteLine("Success: Create succeeded.");
                //Console.WriteLine("Name:    {0}", objGroup.Name);
                //Console.WriteLine("         {0}", objGroup.Properties["displayName"].Value);
                //Console.WriteLine("         {0}", objGroup.Properties["description"].Value);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:   Create failed.");
                Console.WriteLine("         {0}", e.Message);
                //return;
            }


            #endregion

            #region Create User
            DirectoryEntry objUser;  // User object.
            string strUserDisplayName = "Max Bygraves";    // Display name of user.
            string strUserDescription = "A test user"; // Description of the user
            string strUserSAMAccountName = "maxb";  // User's SAMAccountName
            string strUser = "cn=Max Bygraves";         // Group to create.
            Console.WriteLine("Create:  {0}", strUser);

            // Create User.
            try
            {
                objUser = objADAM.Children.Add(strUser, "user");
                objUser.Properties["displayName"].Add(strUserDisplayName);
                objUser.Properties["description"].Add(strUserDescription);
                objUser.Properties["sAMAccountName"].Add(strUserSAMAccountName);
                objUser.CommitChanges();
                objUser.Invoke("SetPassword", new object[] { "SomeStrongPassword:)" });
                objUser.CommitChanges();

                //https://stackoverflow.com/questions/1144966/how-does-the-useraccountcontrol-property-work-in-ad-c
                const int UF_ACCOUNTDISABLE = 0x0002;
                const int UF_PASSWD_NOTREQD = 0x0020;
                const int UF_PASSWD_CANT_CHANGE = 0x0040;
                const int UF_NORMAL_ACCOUNT = 0x0200;
                const int UF_DONT_EXPIRE_PASSWD = 0x10000;
                const int UF_SMARTCARD_REQUIRED = 0x40000;
                const int UF_PASSWORD_EXPIRED = 0x800000;
                //int userControlFlags = UF_PASSWD_NOTREQD + UF_NORMAL_ACCOUNT + UF_DONT_EXPIRE_PASSWD;
                int userControlFlags = UF_NORMAL_ACCOUNT;
                objUser.Properties["userAccountControl"].Value = userControlFlags;
                objUser.CommitChanges();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error:   Create failed.");
                Console.WriteLine("         {0}", e.Message);
                return;
            }


            #endregion

            #region Add User to Group

            Console.WriteLine("Add User to Group:"); 
            try
            {
                string strGroupPath = "LDAP://cn=TestGroup,ou=TestOU,dc=contoso,dc=com";
                string strUserPath = "cn=Max Bygraves,ou=TestOU,dc=contoso,dc=com";
                DirectoryEntry dirEntry = new DirectoryEntry(strGroupPath);
                dirEntry.Properties["member"].Add(strUserPath);
                dirEntry.CommitChanges();
                dirEntry.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException e)
            {
                Console.WriteLine(e.ExtendedErrorMessage);
            }

            #endregion

            #region Remove User From Group
            Console.WriteLine("Remove User from Group:");
            try
            {
                string strGroupPath = "LDAP://cn=TestGroup,ou=TestOU,dc=contoso,dc=com";
                string strUserPath = "cn=Max Bygraves,ou=TestOU,dc=contoso,dc=com";
                DirectoryEntry dirEntry = new DirectoryEntry(strGroupPath);
                dirEntry.Properties["member"].Remove(strUserPath);
                dirEntry.CommitChanges();
                dirEntry.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException e)
            {
                Console.WriteLine(e.ExtendedErrorMessage);
            }

            #endregion

            #region Delete Group
            try
            {
                Console.WriteLine("Delete Group:");

                string strGroupPath = "LDAP://cn=TestGroup,ou=TestOU,dc=contoso,dc=com";
                DirectoryEntry dirEntry = new DirectoryEntry(strGroupPath);
                dirEntry.DeleteTree();
                dirEntry.CommitChanges();
                dirEntry.Close();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException e)
            {
                Console.WriteLine(e.ExtendedErrorMessage);
            }
            #endregion
        }
    }
}

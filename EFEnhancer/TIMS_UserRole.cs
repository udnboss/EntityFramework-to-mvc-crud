//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFEnhancer
{
    using System;
    using System.Collections.Generic;
    
    public partial class TIMS_UserRole
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public Nullable<System.Guid> ProjectID { get; set; }
        public Nullable<System.Guid> ProjectPackageID { get; set; }
        public Nullable<System.Guid> RoleID { get; set; }
    
        public virtual TIMS_Project TIMS_Project { get; set; }
        public virtual TIMS_ProjectPackage TIMS_ProjectPackage { get; set; }
        public virtual TIMS_Role TIMS_Role { get; set; }
        public virtual TIMS_User TIMS_User { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EFEnhancer.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class TIMS_ProjectContractor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIMS_ProjectContractor()
        {
            this.TIMS_ProjectPackage = new HashSet<TIMS_ProjectPackage>();
        }
    
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public Nullable<System.Guid> ProjectID { get; set; }
        public Nullable<System.Guid> ContractorID { get; set; }
    
        public virtual TIMS_Contractor TIMS_Contractor { get; set; }
        public virtual TIMS_Project TIMS_Project { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIMS_ProjectPackage> TIMS_ProjectPackage { get; set; }
    }
}

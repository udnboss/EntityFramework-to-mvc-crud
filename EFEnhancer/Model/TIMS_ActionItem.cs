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
    
    public partial class TIMS_ActionItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIMS_ActionItem()
        {
            this.TIMS_ActionItemWorkflow = new HashSet<TIMS_ActionItemWorkflow>();
            this.TIMS_UserWatchlistItem = new HashSet<TIMS_UserWatchlistItem>();
        }
    
        public System.Guid ID { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIMS_ActionItemWorkflow> TIMS_ActionItemWorkflow { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIMS_UserWatchlistItem> TIMS_UserWatchlistItem { get; set; }
        public virtual TIMS_Project TIMS_Project { get; set; }
        public virtual TIMS_ProjectInterfaceAgreement TIMS_ProjectInterfaceAgreement { get; set; }
        public virtual TIMS_ProjectInterfacePoint TIMS_ProjectInterfacePoint { get; set; }
    }
}

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
    
    public partial class TIMS_ProjectActionItemWorkflow
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TIMS_ProjectActionItemWorkflow()
        {
            this.TIMS_ProjectAttachment = new HashSet<TIMS_ProjectAttachment>();
            this.TIMS_ProjectComment = new HashSet<TIMS_ProjectComment>();
        }
    
        public System.Guid ID { get; set; }
        public string WorkflowTypeID { get; set; }
        public Nullable<System.Guid> ActionItemID { get; set; }
        public Nullable<System.DateTime> DateInitiated { get; set; }
        public string LeadStateID { get; set; }
        public string InterfaceStateID { get; set; }
        public Nullable<System.Guid> UserID { get; set; }
        public Nullable<bool> IsDraft { get; set; }
    
        public virtual TIMS_ProjectActionItem TIMS_ProjectActionItem { get; set; }
        public virtual TIMS_User TIMS_User { get; set; }
        public virtual TIMS_WorkflowType TIMS_WorkflowType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIMS_ProjectAttachment> TIMS_ProjectAttachment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIMS_ProjectComment> TIMS_ProjectComment { get; set; }
    }
}

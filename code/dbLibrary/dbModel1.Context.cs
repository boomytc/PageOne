﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace db
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dbEntities : DbContext
    {
        public dbEntities()
            : base("name=dbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<af_AuditFlow> af_AuditFlow { get; set; }
        public virtual DbSet<af_AuditFlowDept> af_AuditFlowDept { get; set; }
        public virtual DbSet<af_AuditLog> af_AuditLog { get; set; }
        public virtual DbSet<af_AuditNode> af_AuditNode { get; set; }
        public virtual DbSet<af_AuditPost> af_AuditPost { get; set; }
        public virtual DbSet<af_AuditPostUser> af_AuditPostUser { get; set; }
        public virtual DbSet<af_AuditType> af_AuditType { get; set; }
        public virtual DbSet<af_BillCdnField> af_BillCdnField { get; set; }
        public virtual DbSet<af_BillEditField> af_BillEditField { get; set; }
        public virtual DbSet<af_NodeRelation> af_NodeRelation { get; set; }
        public virtual DbSet<bks_Book> bks_Book { get; set; }
        public virtual DbSet<bks_BookStock> bks_BookStock { get; set; }
        public virtual DbSet<bks_BookStockDetail> bks_BookStockDetail { get; set; }
        public virtual DbSet<bks_BookType> bks_BookType { get; set; }
        public virtual DbSet<bks_Customer> bks_Customer { get; set; }
        public virtual DbSet<bks_CustomerAddress> bks_CustomerAddress { get; set; }
        public virtual DbSet<bks_OrderDetail> bks_OrderDetail { get; set; }
        public virtual DbSet<bks_OrderInfo> bks_OrderInfo { get; set; }
        public virtual DbSet<bks_Press> bks_Press { get; set; }
        public virtual DbSet<bks_ShoppingTrolley> bks_ShoppingTrolley { get; set; }
        public virtual DbSet<bks_Supplier> bks_Supplier { get; set; }
        public virtual DbSet<prj_Area> prj_Area { get; set; }
        public virtual DbSet<rbac_Config> rbac_Config { get; set; }
        public virtual DbSet<rbac_DataPriv> rbac_DataPriv { get; set; }
        public virtual DbSet<rbac_Module> rbac_Module { get; set; }
        public virtual DbSet<rbac_Operation> rbac_Operation { get; set; }
        public virtual DbSet<rbac_Resource> rbac_Resource { get; set; }
        public virtual DbSet<rbac_ResourceOp> rbac_ResourceOp { get; set; }
        public virtual DbSet<rbac_Role> rbac_Role { get; set; }
        public virtual DbSet<rbac_RolePriv> rbac_RolePriv { get; set; }
        public virtual DbSet<rbac_RoleUser> rbac_RoleUser { get; set; }
        public virtual DbSet<rbac_User> rbac_User { get; set; }
        public virtual DbSet<rbac_UserOrg> rbac_UserOrg { get; set; }
        public virtual DbSet<rbac_UserPriv> rbac_UserPriv { get; set; }
        public virtual DbSet<sbs_Dept> sbs_Dept { get; set; }
        public virtual DbSet<sbs_Empl> sbs_Empl { get; set; }
        public virtual DbSet<sbs_Org> sbs_Org { get; set; }
        public virtual DbSet<sys_BillAttach> sys_BillAttach { get; set; }
        public virtual DbSet<sys_Column> sys_Column { get; set; }
        public virtual DbSet<sys_DataSyncInterface> sys_DataSyncInterface { get; set; }
        public virtual DbSet<sys_DataSyncInterfaceDetail> sys_DataSyncInterfaceDetail { get; set; }
        public virtual DbSet<sys_UCColumn> sys_UCColumn { get; set; }
        public virtual DbSet<sys_UCPager> sys_UCPager { get; set; }
        public virtual DbSet<sys_WebLog> sys_WebLog { get; set; }
        public virtual DbSet<wx_User> wx_User { get; set; }
        public virtual DbSet<sv_af_AuditCenter> sv_af_AuditCenter { get; set; }
        public virtual DbSet<sv_af_AuditFlow> sv_af_AuditFlow { get; set; }
        public virtual DbSet<sv_af_AuditPostUser> sv_af_AuditPostUser { get; set; }
        public virtual DbSet<sv_bks_Book> sv_bks_Book { get; set; }
        public virtual DbSet<sv_rbac_Resource> sv_rbac_Resource { get; set; }
        public virtual DbSet<sv_rbac_User> sv_rbac_User { get; set; }
        public virtual DbSet<sv_rbac_UserOrg> sv_rbac_UserOrg { get; set; }
        public virtual DbSet<sv_sbs_Dept> sv_sbs_Dept { get; set; }
        public virtual DbSet<sv_sbs_Empl> sv_sbs_Empl { get; set; }
        public virtual DbSet<sv_sys_BillAttach> sv_sys_BillAttach { get; set; }
    }
}

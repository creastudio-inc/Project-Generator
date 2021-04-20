using Infrastructure.Entity;
using ProjectGenerator.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectGenerator.Models
{
    public enum dataFieldType
    {
        Int,
        String,
        Datetime,
        Double,
        Bool,
        Object
    }
    public enum DataAnnotation
    {
        Required,
        DataType,
        Range,
        StringLength,
        MaxLength,
        RegularExpression
    }

    public class ProjectViewModel
    {
        public List<Project> List { get; set; }
        public Pager pager { get; set; }
    }
    public class Project : EntityBase
    {
 

        public string Name { get; set; }
        public string Description { get; set; } 
        public virtual ICollection<Table> TableViewModels { get; set; }
    }

    public class TableViewModel
    {
        public List<Table> List { get; set; }
        public Pager pager { get; set; }
    }
    public class Table : EntityBase
    {
        

        public string Name { get; set; }
        public string Description { get; set; } 

        [ForeignKey("Project")]
        public Guid ProjectID { get; set; }
        public virtual Project Project { get; set; }

        public virtual ICollection<DataFieldForeignKey> dataFieldForeignKeyViewModels { get; set; }

        public virtual ICollection<DataField> DataFieldViewModels { get; set; }
    }
    public class DataFieldViewModel
    {
        public List<DataField> List { get; set; }
        public Pager pager { get; set; }
    }
    public class DataField : EntityBase
    {
 

        public String Name { get; set; }
        public dataFieldType Type { get; set; }
        public String Lengh { get; set; }
        public bool Nullable { get; set; }
        // public bool IsEnum { get; set; }

        [ForeignKey("Table")]
        public Guid TableId { get; set; }
        public virtual Table Table { get; set; }

        public virtual ICollection<DataAnnotationAttrible> DataAnnotationViewModels { get; set; }
    }

    public class DataFieldForeignKeyViewModel
    {
        public List<DataFieldForeignKey> List { get; set; }
        public Pager pager { get; set; }
    }
    public class DataFieldForeignKey : EntityBase
    {
 

        public String Name { get; set; }
        public bool Required { get; set; } 

        [ForeignKey("TableView")]
        public Guid? TableViewID { get; set; }
        public virtual Table TableView { get; set; }
    }
    public class DataAnnotationAttribleViewModel
    {
        public List<DataAnnotationAttrible> List { get; set; }
        public Pager pager { get; set; }
    }
    public class DataAnnotationAttrible : EntityBase
    {
 

        public DataAnnotation Name { get; set; }
        public String Param1 { get; set; }
        public String Param2 { get; set; }
        public String ErrorMessage { get; set; }

        [ForeignKey("DataField")]
        public Guid DataFieldId { get; set; }
        public virtual DataField DataField { get; set; }

    }
    public class EnumViewViewModel
    {
        public List<EnumView> List { get; set; }
        public Pager pager { get; set; }
    }
    public class EnumView : EntityBase
    {
       

        public String Name { get; set; }
        public String Descriptions { get; set; }

        public virtual ICollection<EnumDetail> EnumDetailsViewModels { get; set; }
    }
    public class EnumDetailViewModel
    {
        public List<EnumDetail> List { get; set; }
        public Pager pager { get; set; }
    }
    public class EnumDetail : EntityBase
    {
 
        public String Name { get; set; }
    }
}
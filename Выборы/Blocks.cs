//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Выборы
{
    using System;
    using System.Collections.Generic;
    
    public partial class Blocks
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<int> Option_id { get; set; }
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public int Election_id { get; set; }
    
        public virtual Elections Elections { get; set; }
        public virtual Users Users { get; set; }
    }
}

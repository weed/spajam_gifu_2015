//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはテンプレートから生成されました。
//
//     このファイルを手動で変更すると、アプリケーションで予期しない動作が発生する可能性があります。
//     このファイルに対する手動の変更は、コードが再生成されると上書きされます。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SpajamAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SpotMaster
    {
        public string SpotKey { get; set; }
        public string SpotName { get; set; }
        public string SpotDescription { get; set; }
        public string SpotImageURL { get; set; }
        public int MajorID { get; set; }
        public int MinorID { get; set; }
        public Nullable<long> SortID { get; set; }
        public Nullable<int> PrefectureID { get; set; }
        public Nullable<int> StateCityLineID { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public System.DateTime RegisteredDateTime { get; set; }
    }
}

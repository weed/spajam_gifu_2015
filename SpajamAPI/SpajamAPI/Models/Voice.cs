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
    
    public partial class Voice
    {
        public string TalkID { get; set; }
        public long IndexID { get; set; }
        public string VoiceID { get; set; }
        public string FileID { get; set; }
        public string VoiceAnalysisResult { get; set; }
        public Nullable<long> TextMiningResult { get; set; }
        public string TextMiningResultDetail { get; set; }
        public System.DateTime CreateDateTime { get; set; }
    }
}
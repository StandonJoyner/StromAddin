﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace stromaddin.Resources {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class RibbonResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RibbonResources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("stromaddin.Resources.RibbonResources", typeof(RibbonResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;Root&gt;
        ///  &lt;Params&gt;
        ///    &lt;Param ID=&quot;1001&quot; Name=&quot;Period&quot; Key=&quot;period&quot; Value=&quot;1d&quot;&gt;
        ///      &lt;Value Name=&quot;1 day&quot;  Value=&quot;1d&quot; /&gt;
        ///      &lt;Value Name=&quot;1 week&quot; Value=&quot;1w&quot; /&gt;
        ///      &lt;Value Name=&quot;1 month&quot; Value=&quot;1mo&quot; /&gt;
        ///    &lt;/Param&gt;
        ///  &lt;/Params&gt;
        ///  &lt;Indicators&gt;
        ///    &lt;Indicator Name=&quot;High&quot; Key=&quot;high&quot; Type=&quot;Number&quot; Help=&quot;High price&quot;&gt;
        ///      &lt;Param ID=&quot;1001&quot;/&gt;
        ///    &lt;/Indicator&gt;
        ///    &lt;Indicator Name=&quot;Open&quot; Key=&quot;open&quot; Type=&quot;Number&quot; Help=&quot;Open price&quot;&gt;
        ///      &lt;Param ID=&quot;1001&quot;/&gt;
        ///    &lt; [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string DataHistoryIndicators {
            get {
                return ResourceManager.GetString("DataHistoryIndicators", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap history {
            get {
                object obj = ResourceManager.GetObject("history", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap realtime {
            get {
                object obj = ResourceManager.GetObject("realtime", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap refresh {
            get {
                object obj = ResourceManager.GetObject("refresh", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///&lt;customUI xmlns=&apos;http://schemas.microsoft.com/office/2009/07/customui&apos; loadImage=&apos;LoadImage&apos;&gt;
        ///  &lt;ribbon&gt;
        ///    &lt;tabs&gt;
        ///      &lt;tab id=&apos;stromtab&apos; label=&apos;CoinStrom&apos;&gt;
        ///        &lt;group id=&apos;group1&apos; label=&apos;My Group&apos;&gt;
        ///          &lt;button id=&apos;button1&apos; label=&apos;My Button&apos; onAction=&apos;OnButtonPressed&apos;/&gt;
        ///          &lt;button id=&apos;testBtn&apos; label=&apos;TEST&apos; onAction=&apos;OnTestButtonPressed&apos;/&gt;
        ///          &lt;button id=&apos;rtdBtn&apos; label=&apos;RTD&apos; onAction=&apos;OnRTDButtonPressed&apos;/&gt;
        ///        &lt;/group&gt;
        ///        &lt;gr [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string Ribbon {
            get {
                return ResourceManager.GetString("Ribbon", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt; 
        ///&lt;Root&gt;
        ///  &lt;Params&gt;
        ///    &lt;Param ID=&quot;1000&quot; Name=&quot;Period&quot; Key=&quot;period&quot; Value=&quot;1d&quot;&gt;
        ///      &lt;Value Name=&quot;1 hour&quot; Value=&quot;1h&quot; /&gt;
        ///      &lt;Value Name=&quot;4 hour&quot; Value=&quot;4h&quot; /&gt;
        ///      &lt;Value Name=&quot;1 day&quot;  Value=&quot;1d&quot; /&gt;
        ///    &lt;/Param&gt;
        ///  &lt;/Params&gt;
        ///  &lt;Indicators&gt;
        ///    &lt;Indicator Name=&quot;Time&quot; Key=&quot;time&quot; Type=&quot;Time&quot; Help=&quot;Last event time&quot;&gt;
        ///    &lt;/Indicator&gt;
        ///    &lt;Indicator Name=&quot;Last Price&quot; Key=&quot;last_price&quot; Type=&quot;Number&quot; Help=&quot;Last price&quot;&gt;
        ///    &lt;/Indicator&gt;
        ///    &lt;Indicator Name=&quot;Last [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string RtdIndicators {
            get {
                return ResourceManager.GetString("RtdIndicators", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找 System.Drawing.Bitmap 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Bitmap search {
            get {
                object obj = ResourceManager.GetObject("search", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
    }
}

/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Linq;
using InstaSharper.Classes.Models.TV;
using InstaSharper.Classes.ResponseWrappers.TV;

namespace InstaSharper.Converters.TV
{
    internal class InstaTVConverter : IObjectConverter<InstaTV, InstaTVResponse>
    {
        public InstaTVResponse SourceObject { get; set; }

        public InstaTV Convert()
        {
            if (SourceObject == null)
                throw new ArgumentNullException("SourceObject");

            var tv = new InstaTV
            {
                Status = SourceObject.Status
            };
            if (SourceObject.MyChannel != null)
            {
                try
                {
                    tv.MyChannel = ConvertersFabric.Instance.GetTVSelfChannelConverter(SourceObject.MyChannel).Convert();
                }
                catch { }
            }
            if (SourceObject.Channels != null && SourceObject.Channels.Any())
            {
                foreach (var channel in SourceObject.Channels)
                {
                    try
                    {
                        tv.Channels.Add(ConvertersFabric.Instance.GetTVChannelConverter(channel).Convert());
                    }
                    catch { }
                }
            }

            return tv;
        }
    }
}

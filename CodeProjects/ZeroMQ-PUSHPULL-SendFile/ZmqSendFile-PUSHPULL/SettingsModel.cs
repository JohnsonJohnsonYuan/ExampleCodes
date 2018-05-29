using System;

namespace ZmqSendFile_PUSHPULL
{
    public class SettingsModel
    {
        private static SettingsModel _settingsModel = new SettingsModel();

        static SettingsModel() { }

        private SettingsModel() { }

        /// <summary>
        /// Example:
        /// Address: tcp://*:5557
        /// Port: 5558
        /// tcp://127.0.0.1:5558
        /// tcp://*:5557
        /// </summary>
        public string VentAddress { get; set; }
        public string VentPort { get; set; }
        /// <example>
        /// tcp://*:5557
        /// </example>
        public string VentBind
        {
            get
            {
                return string.Format("tcp://*:{0}", VentPort);
            }
        }
        /// <example>
        /// tcp://127.0.0.1:5558
        /// </example>
        public string VentConnect
        {
            get
            {
                return string.Format("tcp://{0}:{1}", VentAddress, VentPort);
            }
        }

        public string SinkAddress { get; set; }
        public string SinkPort { get; set; }
        /// <example>
        /// tcp://*:5557
        /// </example>
        public string SinkBind
        {
            get
            {
                return string.Format("tcp://*:{0}", SinkPort);
            }
        }
        /// <example>
        /// tcp://127.0.0.1:5558
        /// </example>
        public string SinkConnect
        {
            get
            {
                return string.Format("tcp://{0}:{1}", SinkAddress, SinkPort);
            }
        }

        /// <summary>
        /// Initializes the SettingsModel
        /// </summary>
        public void Initialize()
        {
            var settings = Properties.Settings.Default;

            VentAddress = settings.VentAddress;
            VentPort = settings.VentPort;
            SinkAddress = settings.SinkAddress;
            SinkPort = settings.SinkPort;
        }

        /// <summary>
        /// Gets the instance
        /// </summary>
        public static SettingsModel Instance
        {
            get
            {
                return _settingsModel;
            }
        }
    }
}

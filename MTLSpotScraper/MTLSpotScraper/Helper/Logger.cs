using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace MTLSpotScraper.Helper
{
    public static class Logger
    {
        public static void SetLevel(string loggerName, string levelName)
        {
            ILog log = LogManager.GetLogger(loggerName);
            log4net.Repository.Hierarchy.Logger l = (log4net.Repository.Hierarchy.Logger)log.Logger;
            l.Level = l.Hierarchy.LevelMap[levelName];
        }

        public static IAppender CreateFileAppender(string name, string fileName, string filterValue)
        {
            RollingFileAppender appender = new RollingFileAppender();
            appender.Name = name;
            appender.File = "log\\" + "MTLSpotScraper" + "\\" + fileName;
            appender.AppendToFile = true;
            appender.RollingStyle = RollingFileAppender.RollingMode.Size;

            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = "%d [%t] %-5p %c [%x] - %m%n";
            layout.ActivateOptions();

            appender.Layout = layout;
            if (filterValue == "debug")
                appender.Threshold = Level.Debug;

            if (filterValue == "error")
                appender.Threshold = Level.Error;

            if (filterValue == "fatal")
                appender.Threshold = Level.Fatal;

            appender.RollingStyle = RollingFileAppender.RollingMode.Size;
            appender.MaxSizeRollBackups = 2;
            appender.MaximumFileSize = "2MB";
            appender.ActivateOptions();

            return appender;
        }

        public static void AddAppender2(ILog log, IAppender appender)
        {
            log4net.Repository.Hierarchy.Logger l = (log4net.Repository.Hierarchy.Logger)log.Logger;

            if (l.GetAppender(appender.Name) != null)
                return;

            l.Additivity = false;
            l.AddAppender(appender);
        }

        public static void CreateAppender(ILog logParser, string parserName, string siteCode)
        {
            XmlConfigurator.Configure();

            SetLevel("ParserLog", "DEBUG");

            AddAppender2(logParser, CreateFileAppender(parserName + "DebugAppender", parserName + "-Debug.txt", "debug"));
            AddAppender2(logParser, CreateFileAppender(parserName + "ErrorAppender", parserName + "-Error.txt", "error"));
        }
    }
}
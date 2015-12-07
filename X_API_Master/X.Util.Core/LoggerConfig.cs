﻿using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.IO;

namespace X.Util.Core
{
    public sealed class LoggerConfig
    {
        private static readonly PatternLayout Layout = new PatternLayout(Environment.NewLine + "时间：%date 线程ID：[%thread] 级别：%-5level " + Environment.NewLine + "%logger property:[%property{NDC}] - %message%newline");
        private static readonly IDictionary<string, LevelRangeFilter> Filters = new Dictionary<string, LevelRangeFilter>();
        private static readonly LevelRangeFilter ErrorFilter = new LevelRangeFilter { LevelMax = Level.Error, LevelMin = Level.Error };
        private static readonly LevelRangeFilter InfoFilter = new LevelRangeFilter { LevelMax = Level.Info, LevelMin = Level.Info };
        private static readonly LevelRangeFilter DebugFilter = new LevelRangeFilter { LevelMax = Level.Debug, LevelMin = Level.Debug };
        private static LoggerConfig _instance;
        public static LoggerConfig Instance => _instance ?? (_instance = new LoggerConfig());
        private LoggerConfig()
        {
            ErrorFilter.ActivateOptions();
            Filters["ERROR"] = ErrorFilter;
            InfoFilter.ActivateOptions();
            Filters["INFO"] = InfoFilter;
            DebugFilter.ActivateOptions();
            Filters["DEBUG"] = DebugFilter;
        }

        public ILog GetLogger(LogDomain domain)
        {
            var repository = domain.ToString();
            try
            {
                return LogManager.GetLogger(repository, "default");
            }
            catch
            {
                try
                {
                    var loggerRepository = LogManager.CreateRepository(repository);
                    var log4NetBaseDirectory = AppConfig.Log4NetBaseDirectory;
                    if (string.IsNullOrEmpty(log4NetBaseDirectory))
                    {
                        log4NetBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/../", "Log4NetBaseDirectory");
                    }
                    foreach (var filter in Filters)
                    {
                        var fileAppender = new RollingFileAppender
                        {
                            Name = domain + "_" + filter.Key + "_FileAppender",
                            LockingModel = new FileAppender.MinimalLock(),
                            File = log4NetBaseDirectory,
                            AppendToFile = true,
                            RollingStyle = RollingFileAppender.RollingMode.Date,
                            DatePattern = "/yyyy-MM-dd'/" + domain + "/'yyyy-MM-dd HH'" + filter.Key + ".log'",
                            StaticLogFileName = false,
                            Layout = Layout
                        };
                        fileAppender.AddFilter(filter.Value);
                        fileAppender.ActivateOptions();
                        BasicConfigurator.Configure(loggerRepository, fileAppender);
                    }
                    return LogManager.GetLogger(repository, "default");
                }
                catch
                {
                    return default(ILog);
                }
            }

        }
    }
}

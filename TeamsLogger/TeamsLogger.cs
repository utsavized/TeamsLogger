using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.Teams.Models;
using Newtonsoft.Json;
using TeamsLogger.Models;

namespace TeamsLogger
{
    public class TeamsLogger
    {
        private readonly ITeamsWebhookClient _webhookClient;
        private readonly LoggerConfiguration _loggerConfiguration;
        private readonly string _moduleName;

        private O365ConnectorCard _card;
        private O365ConnectorCardSection _currentSection;
        private bool _hasException;
        private bool _hasWarning;

        public TeamsLogger(ITeamsWebhookClient webhookClient, LoggerConfiguration loggerConfiguration, string moduleName)
        {
            _webhookClient = webhookClient;
            _loggerConfiguration = loggerConfiguration;
            _moduleName = moduleName;
            _hasException = false;
            _hasWarning = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public void LogMessage(LogSeverity severity, string message, string color = null)
        {
            var jsonMsg = GetSerializedMessage(severity, message, color);
            _webhookClient.Post(jsonMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public async Task LogMessageAsync(LogSeverity severity, string message, string color = null)
        {
            var jsonMsg = GetSerializedMessage(severity, message, color);
            await _webhookClient.PostAsync(jsonMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="summary"></param>
        public void BeginRunningLog(string title, string summary = null)
        {
            _card = new O365ConnectorCard(_moduleName, title, summary);
            _currentSection = null;
        }

        /// <summary>
        /// </summary>
        /// <param name="colorHexCode">Color hex code without #, for eg. CCFFE4</param>
        public void PostRunningLog(string colorHexCode = null)
        {
            if (string.IsNullOrEmpty(colorHexCode) && _loggerConfiguration.AutomaticallySetColor)
            {
                if (_hasException)
                    _card.ThemeColor = Defaults.ErrorColor;
                else if (_hasWarning)
                    _card.ThemeColor = Defaults.WarningColor;
            }
            _card.ThemeColor = colorHexCode;

            var jsonPayload = JsonConvert.SerializeObject(_card);
            _webhookClient.Post(jsonPayload);
        }

        /// <summary>
        /// </summary>
        /// <param name="colorHexCode">Color hex code without #, for eg. CCFFE4</param>
        public async void PostRunningLogAsync(string colorHexCode = null)
        {
            if (string.IsNullOrEmpty(colorHexCode) && _loggerConfiguration.AutomaticallySetColor)
            {
                if (_hasException)
                    _card.ThemeColor = Defaults.ErrorColor;
                else if (_hasWarning)
                    _card.ThemeColor = Defaults.WarningColor;
            }
            _card.ThemeColor = colorHexCode;

            var jsonPayload = JsonConvert.SerializeObject(_card);
            await _webhookClient.PostAsync(jsonPayload);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="activityTitle"></param>
        /// <param name="activitySubtitle"></param>
        /// <param name="activityText"></param>
        /// <param name="markdown"></param>
        public void AddNewSection(LogSeverity? severity = null, string title = null, string text = null, string activityTitle = null, string activitySubtitle = null, string activityText = null, bool? markdown = null)
        {
            if (severity.HasValue && severity.Value == LogSeverity.Error)
            {
                _hasException = true;
            }
            if (severity.HasValue && severity.Value == LogSeverity.Warn)
            {
                _hasWarning = true;
            }

            var section = new O365ConnectorCardSection(title, text, activityTitle, activitySubtitle, activityText, null, null, markdown);
            if (!_card.Sections.Any())
            {
                _card.Sections = new List<O365ConnectorCardSection> { section };
            }
            else
            {
                _card.Sections.Add(section);
            }

            _currentSection = section;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkButtonText"></param>
        /// <param name="linkTargetUri"></param>
        public void AddLink(string linkButtonText, string linkTargetUri)
        {
            var link = new O365ConnectorCardOpenUri("OpenUri", linkButtonText, null, new List<O365ConnectorCardOpenUriTarget>
            {
                new O365ConnectorCardOpenUriTarget("default", linkTargetUri)
            });

            if (!_currentSection.PotentialAction.Any())
            {
                _currentSection.PotentialAction = new List<O365ConnectorCardActionBase> { link };
            }
            else
            {
                _currentSection.PotentialAction.Add(link);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="log"></param>
        public void AddSubSectionActivityWithSeverity(LogSeverity severity, string log)
        {
            if (severity == LogSeverity.Error)
            {
                _hasException = true;
            }
            if (severity == LogSeverity.Warn)
            {
                _hasWarning = true;
            }

            var fact = new O365ConnectorCardFact(severity.ToString(), log);
            if (!_currentSection.Facts.Any())
            {
                _currentSection.Facts = new List<O365ConnectorCardFact> { fact };
            }
            else
            {
                _currentSection.Facts.Add(fact);
            }
        }

        private string GetSerializedMessage(LogSeverity severity, string message, string color = null)
        {
            if (string.IsNullOrEmpty(color) && _loggerConfiguration.AutomaticallySetColor)
            {
                switch (severity)
                {
                    case LogSeverity.Info:
                        color = Defaults.InfoColor;
                        break;
                    case LogSeverity.Warn:
                        color = Defaults.WarningColor;
                        break;
                    case LogSeverity.Error:
                        color = Defaults.ErrorColor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
                }
            }
            var teamsMsg = new TeamsMessage { Text = $"[{_moduleName}][{severity}] {message}", ThemeColor = color };
            return JsonConvert.SerializeObject(teamsMsg);
        }
    }
}

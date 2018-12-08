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
        /// Logs simple message with it's severity
        /// </summary>
        /// <param name="severity">Severity of log message</param>
        /// <param name="message">Log message</param>
        /// <param name="color">Optional: Hex code of color of the message card</param>
        public void LogMessage(LogSeverity severity, string message, string color = null)
        {
            var jsonMsg = GetSerializedMessage(severity, message, color);
            _webhookClient.Post(jsonMsg);
        }

        /// <summary>
        /// Logs simple message with it's severity asynchronously
        /// </summary>
        /// <param name="severity">Severity of log message</param>
        /// <param name="message">Log message</param>
        /// <param name="color">Optional: Hex code of color of the message card</param>
        public async Task LogMessageAsync(LogSeverity severity, string message, string color = null)
        {
            var jsonMsg = GetSerializedMessage(severity, message, color);
            await _webhookClient.PostAsync(jsonMsg);
        }

        /// <summary>
        /// Starts a running log
        /// </summary>
        /// <param name="title">Title of the running log card</param>
        /// <param name="summary">Optional: Summary of log card</param>
        public void BeginRunningLog(string title, string summary = null)
        {
            _card = new O365ConnectorCard(_moduleName, title, summary);
            _currentSection = null;
        }

        /// <summary>
        /// Posts running log to Teams
        /// </summary>
        /// <param name="colorHexCode">Optional: Color hex code of the running card (without #, for eg. CCFFE4)</param>
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
        /// Posts running log to Teams asynchoronously
        /// </summary>
        /// <param name="colorHexCode">Optional: Color hex code of the running card (without #, for eg. CCFFE4)</param>
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
        /// Add new section to running log card
        /// </summary>
        /// <param name="severity">Optional: severity of the section</param>
        /// <param name="title">Optional: Title of the section</param>
        /// <param name="text">Optional: Summary of the section</param>
        /// <param name="eventTitle">Optional: Section event title</param>
        /// <param name="eventSubtitle">Optional: Section event subtitle</param>
        /// <param name="eventSummary">Optional: Section event summary</param>
        /// <param name="markdown">If any text has markdown</param>
        public void AddNewSection(LogSeverity? severity = null, string title = null, string text = null, string eventTitle = null, string eventSubtitle = null, string eventSummary = null, bool? markdown = null)
        {
            if (severity.HasValue && severity.Value == LogSeverity.Error)
            {
                _hasException = true;
            }
            if (severity.HasValue && severity.Value == LogSeverity.Warn)
            {
                _hasWarning = true;
            }

            var section = new O365ConnectorCardSection(title, text, eventTitle, eventSubtitle, eventSummary, null, null, markdown);
            if (_card.Sections == null || !_card.Sections.Any())
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
        /// Add link button
        /// </summary>
        /// <param name="linkButtonText">Button text</param>
        /// <param name="linkTargetUri">Target Uri</param>
        public void AddLink(string linkButtonText, string linkTargetUri)
        {
            var link = new O365ConnectorCardOpenUri("OpenUri", linkButtonText, null, new List<O365ConnectorCardOpenUriTarget>
            {
                new O365ConnectorCardOpenUriTarget("default", linkTargetUri)
            });

            if (_currentSection == null)
            {
                _currentSection = new O365ConnectorCardSection();
                _card.Sections = new List<O365ConnectorCardSection> { _currentSection };
            }

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
        /// Add an event within a sub section
        /// </summary>
        /// <param name="severity">Severity of event</param>
        /// <param name="log">Event text</param>
        public void AddSubSectionEvent(LogSeverity severity, string log)
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

            if (_currentSection == null)
            {
                _currentSection = new O365ConnectorCardSection();
                _card.Sections = new List<O365ConnectorCardSection> { _currentSection };
            }

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

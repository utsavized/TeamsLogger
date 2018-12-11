using System;
using System.Threading.Tasks;
using TeamsLogger.Models;

namespace TeamsLogger
{
    public interface ILogger
    {
        /// <summary>
        /// Logs simple message with it's severity
        /// </summary>
        /// <param name="severity">Severity of log message</param>
        /// <param name="message">Log message</param>
        /// <param name="color">Optional: Hex code of color of the message card</param>
        void LogMessage(LogSeverity severity, string message, string color = null);

        /// <summary>
        /// Logs simple message with it's severity asynchronously
        /// </summary>
        /// <param name="severity">Severity of log message</param>
        /// <param name="message">Log message</param>
        /// <param name="color">Optional: Hex code of color of the message card</param>
        Task LogMessageAsync(LogSeverity severity, string message, string color = null);

        /// <summary>
        /// Starts a running log
        /// </summary>
        /// <param name="title">Title of the running log card</param>
        /// <param name="summary">Optional: Summary of log card</param>
        void BeginRunningLog(string title, string summary = null);

        /// <summary>
        /// Posts running log to Teams
        /// </summary>
        /// <param name="colorHexCode">Optional: Color hex code of the running card (without #, for eg. CCFFE4)</param>
        void PostRunningLog(string colorHexCode = null);

        /// <summary>
        /// Posts running log to Teams asynchoronously
        /// </summary>
        /// <param name="colorHexCode">Optional: Color hex code of the running card (without #, for eg. CCFFE4)</param>
        void PostRunningLogAsync(string colorHexCode = null);

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
        void CreateNewMessageCard(LogSeverity? severity = null, string title = null, string text = null, string eventTitle = null, string eventSubtitle = null, string eventSummary = null, bool? markdown = null);

        /// <summary>
        /// Add new exception section to running log card
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="linkToLog">Uri to log file</param>
        /// <param name="logButtonText">Name of link button</param>
        void CreateNewExceptionMessageCard(Exception exception, string linkToLog = null, string logButtonText = null);

        /// <summary>
        /// Add link button
        /// </summary>
        /// <param name="linkButtonText">Button text</param>
        /// <param name="linkTargetUri">Target Uri</param>
        void AddLinkToCurrentMessageCard(string linkTargetUri, string linkButtonText);

        /// <summary>
        /// Add an event within a sub section
        /// </summary>
        /// <param name="severity">Severity of event</param>
        /// <param name="log">Event text</param>
        void AddLogToCurrentMessageCard(LogSeverity severity, string log);
    }
}
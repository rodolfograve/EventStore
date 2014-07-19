using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EventStore.Common.Log;
using EventStore.Common.Utils;
using EventStore.Core.Services.Transport.Http;
using EventStore.Transport.Http;
using EventStore.Transport.Http.Codecs;
using EventStore.Transport.Http.EntityManagement;

namespace EventStore.Core.Util
{
    public class MiniWeb
    {
        private readonly string _localWebRootPath;
        private readonly string _fileSystemRoot;
        private readonly ILogger _logger = LogManager.GetLoggerFor<MiniWeb>();

        public MiniWeb(string localWebRootPath, string fileSystemRoot)
        {
            _logger.Info("Starting MiniWeb for {0} ==> {1}", localWebRootPath, fileSystemRoot);
            _localWebRootPath = localWebRootPath;
            _fileSystemRoot = fileSystemRoot;
        }

        public void RegisterControllerActions(IHttpService service)
        {
            var pattern = _localWebRootPath + "/{*remaining_path}";
            _logger.Trace("Binding MiniWeb to {0}", pattern);
            service.RegisterAction(new ControllerAction(pattern,
                                                                  HttpMethod.Get,
                                                                  Codec.NoCodecs,
                                                                  new ICodec[] { Codec.ManualEncoding }),
                                             OnStaticContent);
        }

        private void OnStaticContent(HttpEntityManager http, UriTemplateMatch match)
        {
            var contentLocalPath = match.BoundVariables["remaining_path"];
            ReplyWithContent(http, contentLocalPath);
        }

        private void ReplyWithContent(HttpEntityManager http, string contentLocalPath)
        {
            //NOTE: this is fix for Mono incompatibility in UriTemplate behavior for /a/b{*C}
            if (("/" + contentLocalPath).StartsWith(_localWebRootPath))
                contentLocalPath = contentLocalPath.Substring(_localWebRootPath.Length);

            //_logger.Trace("{0} requested from MiniWeb", contentLocalPath);
            try
            {
                var extensionToContentType = new Dictionary<string, string>
                {
                    { ".png",  "image/png"} ,
                    { ".jpg",  "image/jpeg"} ,
                    { ".jpeg", "image/jpeg"} ,
                    { ".css",  "text/css"} ,
                    { ".htm",  "text/html"} ,
                    { ".html", "text/html"} ,
                    { ".js",   "application/javascript"} ,
                    { ".json",   "application/json"} ,
                    { ".ico",  "image/vnd.microsoft.icon"}
                };

                var extension = Path.GetExtension(contentLocalPath);
                var fullPath = Path.Combine(_fileSystemRoot, contentLocalPath);

                string contentType;
                if (string.IsNullOrEmpty(extension)
                    || !extensionToContentType.TryGetValue(extension.ToLower(), out contentType)
                    || !File.Exists(fullPath))
                {
                    _logger.Info("Replying 404 for {0} ==> {1}", contentLocalPath, fullPath);
                    http.ReplyTextContent(
                        "Not Found", 404, "Not Found", "text/plain", null, 
                        ex => _logger.InfoException(ex, "Error while replying from MiniWeb"));
                }
                else
                {
                    var config = GetWebPageConfig(contentType);
                    var content = File.ReadAllBytes(fullPath);

                    http.Reply(content,
                                       config.Code,
                                       config.Description,
                                       config.ContentType,
                                       config.Encoding,
                                       config.Headers,
                                       ex => _logger.InfoException(ex, "Error while replying from MiniWeb"));
                }
            }
            catch (Exception ex)
            {
                http.ReplyTextContent(ex.ToString(), 500, "Internal Server Error", "text/plain", null, Console.WriteLine);
            }
        }

        private static ResponseConfiguration GetWebPageConfig(string contentType)
        {
            var encoding = contentType.StartsWith("image") ? null : Helper.UTF8NoBom;
            int? cacheSeconds =
#if RELEASE || CACHE_WEB_CONTENT
                60*60; // 1 hour
#else
                null; // no caching
#endif
// ReSharper disable ExpressionIsAlwaysNull
            return Configure.Ok(contentType, encoding, null, cacheSeconds, isCachePublic: true);
// ReSharper restore ExpressionIsAlwaysNull
        }

        public static string GetWebRootFileSystemDirectory(string debugPath = null)
        {
            string fileSystemWebRoot;
            try
            {
                if (!string.IsNullOrEmpty(debugPath))
                {
                    var sf = new StackFrame(0, true);
                    var fileName = sf.GetFileName();
                    var sourceWebRootDirectory = string.IsNullOrEmpty(fileName)
                                                     ? ""
                                                     : Path.GetFullPath(Path.Combine(fileName, @"..\..\..", debugPath));
                    fileSystemWebRoot = Directory.Exists(sourceWebRootDirectory)
                                            ? sourceWebRootDirectory
                                            : AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    fileSystemWebRoot = AppDomain.CurrentDomain.BaseDirectory;
                }
            }
            catch (Exception)
            {
                fileSystemWebRoot = AppDomain.CurrentDomain.BaseDirectory;
            }
            return fileSystemWebRoot;
        }
    }
}
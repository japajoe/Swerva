using System.Collections.Generic;
using System.Text;

namespace Swerva
{
    public sealed class HttpContentType
    {
        public MediaType type;
        public CharSet charSet;

        private static Dictionary<MediaType, string> mediaTypeToStringTable = new Dictionary<MediaType, string>();
        private static Dictionary<string, MediaType> stringToMediaTypeTable = new Dictionary<string, MediaType>();
        private static Dictionary<CharSet, string> charSetToStringTable = new Dictionary<CharSet, string>();
        private static Dictionary<string, CharSet> stringToCharSetTable = new Dictionary<string, CharSet>();

        public HttpContentType(MediaType type, CharSet charSet = CharSet.UTF8)
        {
            this.type = type;
            this.charSet = charSet;

            if(mediaTypeToStringTable.Count == 0)
            {
                CreateTables();
            }
        }

        public static HttpContentType GetContentTypeFromFileExtension(string filepath)
        {
            return new HttpContentType(GetMediaTypeFromFileExtension(filepath));
        }

        public static MediaType GetMediaTypeFromFileExtension(string filepath)
        {
            MediaType mediaType = MediaType.ApplicationOctetStream;
            
            if(MimeTypeMap.TryGetMimeType(filepath, out string mimeType))
            {
                mediaType = HttpContentType.GetMediaTypeFromString(mimeType);
            }

            return mediaType;
        }

        public static MediaType GetMediaTypeFromString(string contentType)
        {
            if(mediaTypeToStringTable.Count == 0)
            {
                CreateTables();
            }

            if(stringToMediaTypeTable.ContainsKey(contentType))
            {
                return stringToMediaTypeTable[contentType];
            }

            return MediaType.Unknown;
        }

        public static CharSet GetCharSetFromString(string charSet)
        {
            if(mediaTypeToStringTable.Count == 0)
            {
                CreateTables();
            }

            if(stringToCharSetTable.ContainsKey(charSet))
            {
                return stringToCharSetTable[charSet];
            }

            return CharSet.Unknown;
        }

        public override string ToString()
        {
            StringBuilder ss = new StringBuilder();
            ss.Append(mediaTypeToStringTable[type] + "; " + charSetToStringTable[charSet]);
            return ss.ToString();
        }

        private static void CreateTables()
        {
            mediaTypeToStringTable.Add(MediaType.ApplicationEDIX12, "application/EDI-X12");
            mediaTypeToStringTable.Add(MediaType.ApplicationEDIFACT, "application/EDIFACT");
            mediaTypeToStringTable.Add(MediaType.ApplicationJavaScript, "application/javascript");
            mediaTypeToStringTable.Add(MediaType.ApplicationOctetStream, "application/octet-stream");
            mediaTypeToStringTable.Add(MediaType.ApplicationOgg, "application/ogg");
            mediaTypeToStringTable.Add(MediaType.ApplicationPdf, "application/pdf");
            mediaTypeToStringTable.Add(MediaType.ApplicationXhtmlXml, "application/xhtml+xml");
            mediaTypeToStringTable.Add(MediaType.ApplicationXShockWaveFlash, "application/x-shockwave-flash");
            mediaTypeToStringTable.Add(MediaType.ApplicationJson, "application/json");
            mediaTypeToStringTable.Add(MediaType.ApplicationLdJson, "application/ld+json");
            mediaTypeToStringTable.Add(MediaType.ApplicationXml, "application/xml");
            mediaTypeToStringTable.Add(MediaType.ApplicationZip, "application/zip");
            mediaTypeToStringTable.Add(MediaType.ApplicationXWWWFormUrlEncoded, "application/x-www-form-urlencoded");
            mediaTypeToStringTable.Add(MediaType.AudioMpeg, "audio/mpeg");
            mediaTypeToStringTable.Add(MediaType.AudioXMSWma, "audio/x-ms-wma");
            mediaTypeToStringTable.Add(MediaType.AudioVndRnRealAudio, "audio/vnd.rn-realaudio");
            mediaTypeToStringTable.Add(MediaType.AudioXWav, "audio/x-wav");
            mediaTypeToStringTable.Add(MediaType.ImageGif, "image/gif");
            mediaTypeToStringTable.Add(MediaType.ImageJpeg, "image/jpeg");
            mediaTypeToStringTable.Add(MediaType.ImagePng, "image/png");
            mediaTypeToStringTable.Add(MediaType.ImageTiff, "image/tiff");
            mediaTypeToStringTable.Add(MediaType.ImageVndMicrosoftIcon, "image/vnd.microsoft.icon");
            mediaTypeToStringTable.Add(MediaType.ImageXIcon, "image/x-icon");
            mediaTypeToStringTable.Add(MediaType.ImageVndDjvu, "image/vnd.djvu");
            mediaTypeToStringTable.Add(MediaType.ImageSvgXml, "image/svg+xml");
            mediaTypeToStringTable.Add(MediaType.MultiPartMixed, "multipart/mixed");
            mediaTypeToStringTable.Add(MediaType.MultiPartAlternative, "multipart/alternative");
            mediaTypeToStringTable.Add(MediaType.MultiPartRelated, "multipart/related");
            mediaTypeToStringTable.Add(MediaType.MultiPartFormData, "multipart/form-data");
            mediaTypeToStringTable.Add(MediaType.TextCss, "text/css");
            mediaTypeToStringTable.Add(MediaType.TextCsv, "text/csv");
            mediaTypeToStringTable.Add(MediaType.TextHtml, "text/html");
            mediaTypeToStringTable.Add(MediaType.TextJavaScript, "text/javascript");
            mediaTypeToStringTable.Add(MediaType.TextPlain, "text/plain");
            mediaTypeToStringTable.Add(MediaType.TextXml, "text/xml");
            mediaTypeToStringTable.Add(MediaType.VideoMpeg, "video/mpeg");
            mediaTypeToStringTable.Add(MediaType.VideoMp4, "video/mp4");
            mediaTypeToStringTable.Add(MediaType.VideoQuickTime, "video/quicktime");
            mediaTypeToStringTable.Add(MediaType.VideoXMSWmv, "video/x-ms-wmv");
            mediaTypeToStringTable.Add(MediaType.VideoXMSVideo, "video/x-msvideo");
            mediaTypeToStringTable.Add(MediaType.VideoXFlv, "video/x-flv");
            mediaTypeToStringTable.Add(MediaType.VideoWebM, "video/webm");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndOasisOpenDocumentText, "application/vnd.oasis.opendocument.text");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndOasisOpenDocumentSpreadSheet, "application/vnd.oasis.opendocument.spreadsheet");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndOasisOpenDocumentPresentation, "application/vnd.oasis.opendocument.presentation");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndOasisOpenDocumentGraphics, "application/vnd.oasis.opendocument.graphics");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndMSExcel, "application/vnd.ms-excel");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndOpenXmlFormatsOfficeDocumentSpreadSheetMLSheet, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndMSPowerpoint, "application/vnd.ms-powerpoint");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndOpenXmlFormatsOfficeDocumentPresentationMLPresentation, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            mediaTypeToStringTable.Add(MediaType.ApplicationMSWord, "application/msword");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndOpenXmlFormatsOfficeDocumentWordProcessingMLDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            mediaTypeToStringTable.Add(MediaType.ApplicationVndMozillaXulXml, "application/vnd.mozilla.xul+xml");
            mediaTypeToStringTable.Add(MediaType.Unknown, "unknown");

            foreach(var item in mediaTypeToStringTable)
            {
                stringToMediaTypeTable.Add(item.Value, item.Key);
            }

            charSetToStringTable.Add(CharSet.ISO88591, "charset=ISO-8859-1");
            charSetToStringTable.Add(CharSet.ISO88598, "charset=ISO-8859-8");
            charSetToStringTable.Add(CharSet.UTF8, "charset=utf-8");
            charSetToStringTable.Add(CharSet.UTF16, "charset=utf-16");
            charSetToStringTable.Add(CharSet.Unknown, "charset=unknown");

            foreach(var item in charSetToStringTable)
            {
                stringToCharSetTable.Add(item.Value, item.Key);
            }
        }
    }

    public enum MediaType
    {
        ApplicationEDIX12,
        ApplicationEDIFACT,
        ApplicationJavaScript,
        ApplicationOctetStream,
        ApplicationOgg,
        ApplicationPdf,
        ApplicationXhtmlXml,
        ApplicationXShockWaveFlash,
        ApplicationJson,
        ApplicationLdJson,
        ApplicationXml,
        ApplicationZip,
        ApplicationXWWWFormUrlEncoded,
        AudioMpeg,
        AudioXMSWma,
        AudioVndRnRealAudio,
        AudioXWav,
        ImageGif,
        ImageJpeg,
        ImagePng,
        ImageTiff,
        ImageVndMicrosoftIcon,
        ImageXIcon,
        ImageVndDjvu,
        ImageSvgXml,
        MultiPartMixed,
        MultiPartAlternative,
        MultiPartRelated,
        MultiPartFormData,
        TextCss,
        TextCsv,
        TextHtml,
        TextJavaScript,
        TextPlain,
        TextXml,
        VideoMpeg,
        VideoMp4,
        VideoQuickTime,
        VideoXMSWmv,
        VideoXMSVideo,
        VideoXFlv,
        VideoWebM,
        ApplicationVndOasisOpenDocumentText,
        ApplicationVndOasisOpenDocumentSpreadSheet,
        ApplicationVndOasisOpenDocumentPresentation,
        ApplicationVndOasisOpenDocumentGraphics,
        ApplicationVndMSExcel,
        ApplicationVndOpenXmlFormatsOfficeDocumentSpreadSheetMLSheet,
        ApplicationVndMSPowerpoint,
        ApplicationVndOpenXmlFormatsOfficeDocumentPresentationMLPresentation,
        ApplicationMSWord,
        ApplicationVndOpenXmlFormatsOfficeDocumentWordProcessingMLDocument,
        ApplicationVndMozillaXulXml,
        Unknown
    }

    public enum CharSet
    {
        ISO88591,
        ISO88598,
        UTF8,
        UTF16,
        Unknown
    }
}
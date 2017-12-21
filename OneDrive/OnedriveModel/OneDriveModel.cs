using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OneDrive.OnedriveModel
{
    #region JSON实列
    /*
     * 这个类通过
     * https://www.bejson.com/convert/json2csharp/
     * 生成获得
     * {
	"@odata.context": "https://graph.microsoft.com/v1.0/$metadata#users('seaqi65%40163.com')/drive/items('1C1370877CDEA235%21104')/children",
	"value": [
		{
			"createdBy": {
				"application": {
					"displayName": "MSOffice15",
					"id": "480728c5"
				},
				"device": {
					"id": "184000aff736d4"
				},
				"user": {
					"displayName": "\u9093\u5174\u6d77",
					"id": "1c1370877cdea235"
				},
				"oneDriveSync": {
					"@odata.type": "#microsoft.graph.identity",
					"id": "61abe7b1-38bb-4de9-9946-12239ec47008"
				}
			},
			"createdDateTime": "2016-04-20T05:33:50.193Z",
			"cTag": "adDoxQzEzNzA4NzdDREVBMjM1ITU0NC42MzU5NjcyNzIzMTAxNzAwMDA",
			"eTag": "aMUMxMzcwODc3Q0RFQTIzNSE1NDQuMQ",
			"id": "1C1370877CDEA235!544",
			"lastModifiedBy": {
				"application": {
					"displayName": "MSOffice15",
					"id": "480728c5"
				},
				"device": {
					"id": "184000aff736d4"
				},
				"user": {
					"displayName": "\u9093\u5174\u6d77",
					"id": "1c1370877cdea235"
				}
			},
			"lastModifiedDateTime": "2016-04-20T05:33:51.017Z",
			"name": "Camera Roll",
			"parentReference": {
				"driveId": "1c1370877cdea235",
				"id": "1C1370877CDEA235!104",
				"name": "\u56fe\u7247",
				"path": "/drive/root:/%E5%9B%BE%E7%89%87"
			},
			"size": 0,
			"webUrl": "https://1drv.ms/f/s!ADWi3nyHcBMchCA",
			"fileSystemInfo": {
				"createdDateTime": "2016-04-20T05:33:39Z",
				"lastModifiedDateTime": "2016-04-20T05:33:51.016Z"
			},
			"folder": {
				"childCount": 0,
				"view": {
					"viewType": "thumbnails",
					"sortBy": "takenOrCreatedDateTime",
					"sortOrder": "descending"
				}
			},
			"specialFolder": {
				"name": "cameraRoll"
			}
		}
	]
}
     */
    #endregion

    #region OneDrive Search Model
    [DataContract]
    public class ParentReference
    {
        [DataMember(Name = "driveId")]
        public string driveId { get; set; }

        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }


        [DataMember(Name = "path")]
        public string path { get; set; }
    }

    [DataContract]
    public class View
    {
        [DataMember(Name = "viewType")]
        public string viewType { get; set; }

        [DataMember(Name = "sortBy")]
        public string sortBy { get; set; }

        [DataMember(Name = "sortOrder")]
        public string sortOrder { get; set; }
    }

    [DataContract]
    public class FolderDate
    {
        [DataMember(Name = "childCount")]
        public int childCount { get; set; }

        [DataMember(Name = "view")]
        public View view { get; set; }
    }

    [DataContract]
    public class Hashes
    {
        [DataMember(Name = "sha1Hash")]
        public string sha1Hash { get; set; }
    }

    [DataContract]
    public class FileDate
    {
        [DataMember(Name = "hashes")]
        /// <summary>
        /// File hashes value
        /// </summary>
        public Hashes hashes { get; set; }

        [DataMember(Name = "mimeType")]
        /// <summary>
        /// mime type
        /// </summary>
        public string mimeType { get; set; }
    }

    [DataContract]
    public class ValueItem
    {
        [DataMember(Name = "createdDateTime")]
        /// <summary>
        /// create date
        /// </summary>
        public string createdDateTime { get; set; }

        [DataMember(Name = "cTag")]
        /// <summary>
        /// tag describe
        /// </summary>
        public string cTag { get; set; }

        [DataMember(Name = "eTag")]
        /// <summary>
        /// tag describe
        /// </summary>
        public string eTag { get; set; }

        [DataMember(Name = "id")]
        /// <summary>
        /// item id - uniqueness
        /// </summary>
        public string id { get; set; }

        [DataMember(Name = "lastModifiedDateTime")]
        /// <summary>
        /// last modified date
        /// </summary>
        public string lastModifiedDateTime { get; set; }

        [DataMember(Name = "name")]
        /// <summary>
        /// item name
        /// </summary>
        public string name { get; set; }

        [DataMember(Name = "parentReference")]
        /// <summary>
        /// parent describe
        /// </summary>
        public ParentReference parentReference { get; set; }

        [DataMember(Name = "size")]
        /// <summary>
        /// 大小
        /// </summary>
        public int size { get; set; }

        [DataMember(Name = "webUrl")]
        public string webUrl { get; set; }

        [DataMember(Name = "folder")]
        public FolderDate folder { get; set; }

        [DataMember(Name = "file")]
        public FileDate file { get; set; }
    }


    [DataContract]
    public class CatalogData
    {
        [DataMember(Name = "@odata.context")]
        /// <summary>
        /// 请求的用户名和密码上下文对象
        /// </summary>
        public string datacontext { get; set; }

        [DataMember(Name = "value")]
        /// <summary>
        /// 返回的children对象集合
        /// </summary>
        public List<ValueItem> value { get; set; }
    }
    #endregion

    #region OneDrive Create Model
    [DataContract]
    public class FolderData
    {
    }

    [DataContract]
    public class CreateFolderModel
    {
        [DataMember(Name = "folder")]
        /// <summary>
        /// 
        /// </summary>
        public FolderData folder { get; set; }

        [DataMember(Name = "name")]
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
    }
    #endregion


}

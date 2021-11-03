using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Utilities;

namespace NbSites.LogCenter.Api.Logs.AppServices
{
    public interface ILogFileService: IAutoInjectAsTransient
    {
        List<string> SearchLogFiles(SearchLogFilesArgs args);
        MessageResult DeleteLogFiles(DeleteLogFilesArgs args);
        Task<MessageResult> ReadLogFile(string fileId);
    }
    
    public class SearchLogFilesArgs
    {
        public string Name { get; set; }
    }

    public class DeleteLogFilesArgs
    {
        public List<string> FileIds { get; set; } = new List<string>();
    }

    public class LogFileService : ILogFileService
    {
        public List<string> SearchLogFiles(SearchLogFilesArgs args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            var fileHelper = FileHelper.Instance;
            var logDirectory = fileHelper.CombineBaseDirectory("_nlogs");
            var groupNameFiles = GroupNameFile.GetFiles(logDirectory, true, "*.log").ToGroupNameFiles(logDirectory);
            var files = groupNameFiles.Search(GroupNameSearchArgs.Create("/", args.Name, true)).OrderByGroupName().Select(x => x.GetFullGroupNameWithExtension()).ToList();
            return files;
        }

        public MessageResult DeleteLogFiles(DeleteLogFilesArgs args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            var messageResult = new MessageResult();
            if (args.FileIds.IsNullOrEmpty())
            {
                messageResult.Message = "没有要删除的文件";
                return messageResult;
            }

            var deleteRun = false;
            var deleteLogs = new List<string>();
            var fileHelper = FileHelper.Instance;
            var groupNameFiles = args.FileIds.Select(GetGroupNameFile).ToList();
            foreach (var groupNameFile in groupNameFiles)
            {
                var fullFilePath = groupNameFile.GetFullFilePath();
                if (fileHelper.Exists(fullFilePath))
                {
                    fileHelper.Delete(fullFilePath);
                    deleteRun = true;
                    deleteLogs.Add($"found and deleted: {groupNameFile.GetFullGroupNameWithExtension()}");
                }
                else
                {
                    deleteLogs.Add($"not found: {groupNameFile.GetFullGroupNameWithExtension()}");
                }
            }

            messageResult.Success = deleteRun;
            messageResult.Message = "删除已执行";
            messageResult.Data = deleteLogs.OrderBy(x => x).ToList();

            return messageResult;
        }

        public async Task<MessageResult> ReadLogFile(string fileId)
        {
            var messageResult = new MessageResult();

            var groupNameFile = GetGroupNameFile(fileId);
            if (groupNameFile == null)
            {
                messageResult.Message = "文件不存在:" + fileId;
                return messageResult;
            }
            
            var fileHelper = FileHelper.Instance;
            var filePath = groupNameFile.GetFullFilePath();
            if (!fileHelper.Exists(filePath))
            {
                messageResult.Message = $"文件不存在: {groupNameFile.GetFullGroupNameWithExtension()}";
                return messageResult;
            }

            var contents = await fileHelper.ReadFileAsync(filePath);
            messageResult.Data = contents;
            messageResult.Success = true;
            messageResult.Message = "OK";
            return messageResult;
        }

        private GroupNameFile GetGroupNameFile(string fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return null;
            }
            
            //修正转义
            fileId = fileId.Replace("%2F", "/");
            if (!fileId.EndsWith(".log"))
            {
                fileId = fileId + ".log";
            }

            if (!fileId.EndsWith(".log"))
            {
                fileId += ".log";
            }
            
            // "1.log"
            // "remotes/1.log"
            if (fileId.StartsWith('/'))
            {
                fileId = fileId.TrimStart('/');
            }

            var fileHelper = FileHelper.Instance;
            var logDirectory = fileHelper.CombineBaseDirectory("_nlogs");
            var groupNameFile = GroupNameFile.Create(logDirectory, fileId);
            return groupNameFile;
        }
    }
}

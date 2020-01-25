using System.Collections.Immutable;
using System.Threading.Tasks;
using MessageApiGateway.Controllers;

namespace MessageApiGateway
{
    [Export(typeof(ITranspilerProvider))]
    internal class TranspileService : ITranspilerProvider
    {
        private readonly IProcessService _processService;
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;

        [ImportingConstructor]
        public TranspileService(IProcessService processService, IDirectoryService directoryService, IFileService fileService)
        {
            _processService = processService;
            _directoryService = directoryService;
            _fileService = fileService;
        }

        public string Name { get; } = "Local";

        public Task<ImmutableList<TranspileTargetLanguage>> GetSupportedTranspileLanguages()
        {
            var builder = ImmutableList.CreateBuilder<TranspileTargetLanguage>();
            builder.Add(new TranspileToCSharp());
            var immutableList = builder.ToImmutable();

            return Task.FromResult(immutableList);
        }

        public async Task<TranspileResult> TranspileToAsync(ProtoFile protoFile, TranspileTargetLanguage transpileTargetLanguage)
        {
            var fileExecute = _fileService.GetFileInfo("protoc.exe");
            var targetFile = _fileService.GetFileInfo(protoFile.FullName.Replace(".proto", transpileTargetLanguage.FileExtension));

            var protoFileInfo = _fileService.GetFileInfo(protoFile.FullName);

            var arguments = $"-I={protoFileInfo.Directory.FullName} --{transpileTargetLanguage.LanguageName}={targetFile.Directory.FullName} {protoFileInfo.FullName}";

            var exitCode = await _processService.StartProcessAsync(fileExecute, arguments);
            if (exitCode != 0)
            {
                throw new StartProcessException($"Start process or run process: '{fileExecute} {arguments}' exit with error code: '{exitCode}'");
            }

            Throw.If(() => targetFile, f => f.Exists.IsFalse(), $"Expected transpile result file:'{targetFile.FullName}' does not exists.");

            return new TranspileResult(protoFile, targetFile.ReadAllText(), transpileTargetLanguage);
        }
    }
}

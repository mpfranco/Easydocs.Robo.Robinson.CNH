using RestSharp;
using Easydocs.Robo.Robinson.CNH.Application.UseCases.Queries.Occurrences;
using Easydocs.Robo.Robinson.CNH.Domain.Services;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Comunication;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Services;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Helpers;
using System.Linq;
using Polly;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text;
using ClosedXML.Excel;
using Easydocs.Robo.Robinson.CNH.Infrastructure.Shared.Extensions;
using Easydocs.Robo.Robinson.CNH.Domain.Dto;
using System.Threading;
using OpenQA.Selenium;

namespace Easydocs.Robo.Robinson.CNH.Application.UseCases.Services
{
    public class FindProcessoService : IFindProcerssoService
    {
        SeleniumHelper selenium;        
        private readonly RoboRobinsonSettings _roboRobinsonSettings;        
        private readonly ILoggerRomaneio _logger;        
        string directoryProcessed = "";
        string directoryProcessedUnified = "";
        string diretorioProcesso = DateTime.Now.ToString("yyyyMMdd");
        string nomeExcelUpload = DateTime.Now.ToString("yyyyMMdd HHmmss");
        List<FileInfo> fileNaoProcessados;
        public FindProcessoService(ILoggerRomaneio logger,RoboRobinsonSettings roboRobinsonSettings)
        {
            _roboRobinsonSettings = roboRobinsonSettings;
            selenium = new SeleniumHelper(_roboRobinsonSettings.diretorioTemp);            
            _logger = logger;
            directoryProcessed = _roboRobinsonSettings.diretorioArquivosProcessados;
            directoryProcessedUnified = _roboRobinsonSettings.diretorioArquivosUnificados;
        }
        void WaitAndRetry()
        {
            var waitRetry = Policy
                              .Handle<Exception>()
                              .WaitAndRetryAsync(50, i => TimeSpan.FromSeconds(10), (result, timeSpan, retryCount, context) =>
                              {
                                  Console.WriteLine($"Request failed. Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                              });

            waitRetry.ExecuteAsync(async () =>
            {
                await readFile();
            });
        }
        public async Task Executar()
        {
            WaitAndRetry();
        }
        private async Task readFile()
        {

            try
            {

                var processos = carregarProcessos();
                fileNaoProcessados = new List<FileInfo>();

                if (!processos.Any())
                {
                    LogInformation("Não foram encontrados processos para envio.");
                }
                else
                {
                    LogInformation("Acessando Portal CNH");

                    if (!login(_roboRobinsonSettings.user, _roboRobinsonSettings.password))
                    {
                        LogError("Não foi possível efetuar login!");
                        return;
                    }

                    //Acessando menu cadastro de documentos
                    selenium.ClicarPorXPath("//*[@id='main']/div[1]/nav/ul/li[2]/a");
                    Thread.Sleep(1000);
                    selenium.ClicarPorLinkText("Cadastro de Documentos");

                    //Envia planilha excel com os dados
                    selenium.PreencherTextBoxPorId("relatorio", $"{directoryProcessedUnified}\\{diretorioProcesso}\\{nomeExcelUpload}.xlsx");
                    selenium.ClicarPorId("add-relatorio");

                    Thread.Sleep(1000);
                    if (!planilhaContemErros())
                    {
                        var fileUpload = selenium.ObterElementoVisivelPorId("notasfiscais");
                        var arquivos = "";

                        foreach (var processo in processos)
                        {
                            if (arquivos.Length > 0) arquivos += "\n";
                            arquivos += $"{processo.ArquivoNFUnificado}\n{processo.ArquivoFaturaUnificado}";
                        }

                        fileUpload.SendKeys(arquivos);
                        Thread.Sleep(2000);
                        
                        selenium.ClicarPorXPath("//*[@id='fnComexCadastro']/div[1]/div[10]/div/div/div[3]/div[2]/a");
                        Thread.Sleep(2000);

                        var divLoading = selenium.ObterElementoPorId("loading");
                        var divDisplay = "";

                        do
                        {
                            divDisplay = divLoading.GetAttribute("style");

                        } while (divDisplay.Equals("display: block;"));

                        if (_roboRobinsonSettings.salvarEnvio?.ToUpper() == "S")
                        {
                            selenium.ClicarPorId("btn-salvar");
                            Thread.Sleep(1000);
                            fecharPoupSucesso();

                            MoverArquivosProcessados(processos);

                            if (fileNaoProcessados.Any()) MoverArquivosNaoProcessados(fileNaoProcessados);
                        }
                                               
                    }

                    selenium.Dispose();

                    LogInformation("Finalizado.");

                }

            }
            catch (Exception err)
            {
                LogError(err.Message);
                throw new Exception(err.ToString());
            }



        }

        private void fecharPoupSucesso()
        {
            try
            {
                // class div = bootbox modal fade bootbox-alert in
                var corpo = selenium.ObterElementoPorId("corpo");
                var divModalSucesso = corpo.FindElement(By.XPath("//*[@id='corpo']/div[6]"));
                var divStyle = "";

                do          
                {
                    divModalSucesso = corpo.FindElement(By.XPath("//*[@id='corpo']/div[6]"));

                } while (divModalSucesso is null);
                
                selenium.ClicarPorXPath("//*[@id='corpo']/div[6]/div/div/div[3]/button");
            }
            catch (Exception err)
            {
                LogError(err.Message);
            }
        }
        private bool planilhaContemErros()
        {
            var retorno = true;
            try
            {

                var tabelaDocumentos = selenium.ObterElementoPorId("table-documentos");

                var tabelaCorpo = tabelaDocumentos.FindElement(By.TagName("tbody"));

                var quantidadesLinhas = 0;

                var modalErro = selenium.ObterElementoPorIdExistente("modal-erros");

                var errorList = "";

                do
                {
                    quantidadesLinhas = tabelaCorpo.FindElements(By.TagName("tr")).Count;
                    errorList = modalErro.FindElement(By.Id("error-list"))?.Text;

                } while (quantidadesLinhas <= 0 && errorList.Length <= 0);


                if (quantidadesLinhas > 0)
                    retorno = false;
                else
                {
                    foreach (var erro in errorList.Split('\n'))
                    {
                        LogError(erro);
                    }

                    selenium.ClicarPorXPath("//*[@id='modal-erros']/div/div/div[3]/button");
                }

                return retorno;
            }
            catch (Exception err)
            {
                return false;
            }
        }
        public bool login(string user, string password)
        {
            try
            {
                selenium.acessarUrl(_roboRobinsonSettings.urlBase);
                selenium.PreencherTextBoxPorId("Login", user);
                selenium.PreencherTextBoxPorId("Senha", password);
                selenium.ClicarPorId("btn-autenticar");
            }
            catch (Exception err)
            {
                LogError(err.Message);
                return false;
            }

            return true;
        }

        private List<ProcessoDto> carregarProcessos()
        {
            var processos = new List<ProcessoDto>();
            var directory = new DirectoryInfo(_roboRobinsonSettings.diretorioArquivosProcessar);
            var files = directory.GetFiles("*.pdf*", SearchOption.AllDirectories);
            var filesGroupedByName = files.GroupBy(file => file.Name.Substring(0, file.Name.IndexOf("-")) + file.Name.Substring(file.Name.IndexOf("-"), 3));
            var wb = new XLWorkbook(_roboRobinsonSettings.diretorioTemplate);
            var worksheet = wb.Worksheet("Sheet1");

            var row = 2;

            foreach (var filegroup in filesGroupedByName)
            {
                if (filegroup?.Count() == 3)
                {
                    var imagemFatura = filegroup.FirstOrDefault(f => f.Name.ToUpper().EndsWith("F.PDF"));
                    var imagemNotaFiscal = filegroup.FirstOrDefault(f => f.Name.ToUpper().Contains("NF"));
                    var imagemHouse = filegroup.FirstOrDefault(f => f.Name.ToUpper().EndsWith("H.PDF"));
                    var dadosNF = imagemNotaFiscal.Name.ToString().Split('_');
                    var valorNf = "";
                    var valorFatura = "";

                    if (imagemFatura == null)
                    {
                        fileNaoProcessados.AddRange(filegroup);
                        continue;
                    }
                    var pdfReader = new PdfReader(imagemFatura.FullName);
                    var pdfText = PdfTextExtractor.GetTextFromPage(pdfReader, 1).ToString();
                    var linhas = pdfText.Split('\n');

                    if (pdfReader.NumberOfPages > 1)
                    {
                        pdfText = PdfTextExtractor.GetTextFromPage(pdfReader, pdfReader.NumberOfPages).ToString();
                        var linhasPag2 = pdfText.Split('\n');
                        valorFatura = linhasPag2.FirstOrDefault(l => l.ToString().ToUpper().Contains("TOTAL")).Split(')')?[1]?.Trim();
                    }
                    else
                    {
                        valorFatura = linhas.FirstOrDefault(l => l.ToString().ToUpper().Contains("TOTAL")).Split(')')?[1]?.Trim();
                    }


                    var nrFatura = linhas.FirstOrDefault(l => l.ToString().ToUpper().Contains("FATURA - Nº:"))?.Split(':')?[1]?.Trim();
                    var nrNotaFiscal = dadosNF[1];
                    valorNf = dadosNF[2]?.Substring(0, dadosNF[2].IndexOf("."));
                    var charSplitValor = valorNf.Contains(".") ? "." : ",";
                    var processoSplit = linhas.FirstOrDefault(l => l.ToString().ToUpper().Contains("Nº PROCESSO:"))?.Split(':')?[3]?.Trim().Split('/');
                    var nrProcesso = $"{processoSplit[0]}{processoSplit[1]}";
                    var processo = new ProcessoDto
                    {                        
                        NrProcesso = nrProcesso,
                        NrFatura = nrFatura.OnlyNumbers(),
                        House = linhas.FirstOrDefault(l => l.ToString().ToUpper().Contains("HOUSE:"))?.Split(':')?[2]?.Trim(),
                        DtEmissao = linhas.FirstOrDefault(l => l.ToString().ToUpper().Contains("DATA DE EMISSÃO:"))?.Split(':')?[1]?.Trim()?.Substring(0, 10),
                        CnpjFornecedor = linhas.FirstOrDefault(l => l.ToString().ToUpper().Contains("CNPJ:"))?.Split(':')?[1]?.OnlyNumbers(),
                        DtVencimentoFatura = linhas.FirstOrDefault(l => l.ToString().ToUpper().Contains("DATA DE VENCIMENTO:"))?.Split(':')?[2]?.Trim()?.Substring(0, 10),
                        CnpjEmpresa = linhas.Where(l => l.ToString().ToUpper().Contains("CNPJ:")).ToList()?[1]?.Split(':')?.Last()?.OnlyNumbers(),
                        NrNotaFiscal = nrNotaFiscal,
                        ValorNf = valorNf.Split(charSplitValor)?.Count() == 2 ? valorNf : valorNf + ",00",
                        ValorFatura = valorFatura,
                        ArquivoFatura = imagemFatura.FullName,
                        ArquivoNF = imagemNotaFiscal.FullName,
                        ArquivoHouse = imagemHouse.FullName,
                        ArquivoFaturaUnificado = $"{directoryProcessedUnified}\\{diretorioProcesso}\\{nrProcesso}.pdf",
                        ArquivoNFUnificado = $"{directoryProcessedUnified}\\{diretorioProcesso}\\{nrNotaFiscal}.pdf"
                    };


                    if (!Directory.Exists($"{directoryProcessedUnified}\\{diretorioProcesso}")) 
                        Directory.CreateDirectory($"{directoryProcessedUnified}\\{diretorioProcesso}");

                    unificarPdfs(filegroup.ToList(), processo.ArquivoFaturaUnificado);
                    unificarPdfs(filegroup.ToList(), processo.ArquivoNFUnificado);

                    worksheet.Cell($"B{row}").Value = "Importação";
                    worksheet.Cell($"E{row}").Value = processo.DtEmissao.Split('/')?.Last();
                    worksheet.Cell($"H{row}").Value = processo.House.Substring(1, processo.House.Length - 1);

                    worksheet.Cell($"J{row}").Value = processo.NrProcesso;
                    worksheet.Cell($"J{row}").DataType = XLDataType.Text;

                    worksheet.Cell($"K{row}").Value = processo.ValorFatura;

                    worksheet.Cell($"L{row}").Style.NumberFormat.Format = "@";
                    worksheet.Cell($"L{row}").DataType = XLDataType.Text;
                    worksheet.Cell($"L{row}").Value = $@"{processo.CnpjFornecedor}";


                    worksheet.Cell($"M{row}").Value = processo.DtEmissao;
                    worksheet.Cell($"N{row}").Value = "BRL";
                    worksheet.Cell($"O{row}").Value = processo.DtVencimentoFatura;

                    worksheet.Cell($"Q{row}").Style.NumberFormat.Format = "@";
                    worksheet.Cell($"Q{row}").DataType = XLDataType.Text;
                    worksheet.Cell($"Q{row}").Value = $@"{processo.CnpjEmpresa}";



                    row++;

                    worksheet.Cell($"B{row}").Value = "Importação";
                    worksheet.Cell($"E{row}").Value = processo.DtEmissao.Split('/')?.Last();
                    worksheet.Cell($"H{row}").Value = processo.House.Substring(1, processo.House.Length - 1);

                    worksheet.Cell($"J{row}").Value = processo.NrNotaFiscal;
                    worksheet.Cell($"J{row}").DataType = XLDataType.Text;

                    worksheet.Cell($"K{row}").Value = processo.ValorNf;

                    worksheet.Cell($"L{row}").Style.NumberFormat.Format = "@";
                    worksheet.Cell($"L{row}").DataType = XLDataType.Text;
                    worksheet.Cell($"L{row}").Value = $@"{processo.CnpjFornecedor}";


                    worksheet.Cell($"M{row}").Value = processo.DtEmissao;
                    worksheet.Cell($"N{row}").Value = "BRL";
                    worksheet.Cell($"O{row}").Value = processo.DtVencimentoFatura;


                    worksheet.Cell($"Q{row}").Style.NumberFormat.Format = "@";
                    worksheet.Cell($"Q{row}").DataType = XLDataType.Text;
                    worksheet.Cell($"Q{row}").Value = $@"{processo.CnpjEmpresa}";


                    row++;

                    worksheet.ExpandColumns();
                    worksheet.ExpandRows();

                    processos.Add(processo);
                }
                else
                {
                    fileNaoProcessados.AddRange(filegroup);
                }
            }

            wb.SaveAs($"{directoryProcessedUnified}\\{ diretorioProcesso}\\{nomeExcelUpload}.xlsx");
            wb.Dispose();

            return processos;
        }


        private void unificarPdfs(List<FileInfo> files, string outputFile)
        {

            PdfImportedPage importedPage;


            var sourceDocument = new Document();
            var pdfCopyProvider = new PdfCopy(sourceDocument, new FileStream(outputFile, FileMode.Create));

            sourceDocument.Open();

            try
            {
                foreach (var file in files)
                {
                    var reader = new PdfReader(file.FullName);
                    for (int pag = 1; pag <= reader.NumberOfPages; pag++)
                    {
                        importedPage = pdfCopyProvider.GetImportedPage(reader, pag);
                        pdfCopyProvider.AddPage(importedPage);
                    }

                    reader.Close();
                }

                sourceDocument.Close();
            }
            catch (Exception err)
            {
                LogError(err.Message);
                throw new Exception(err.Message);
            }
            finally
            {
                if (sourceDocument.IsOpen()) sourceDocument.Close();
            }
        }

        private void MoverArquivosProcessados(List<ProcessoDto> processos)
        {
            if (!Directory.Exists($"{directoryProcessed}\\{diretorioProcesso}")) Directory.CreateDirectory($"{directoryProcessed}\\{diretorioProcesso}");

            foreach (var processo in processos)
            {
                File.Copy(processo.ArquivoFatura, $"{directoryProcessed}\\{diretorioProcesso}\\{new FileInfo(processo.ArquivoFatura).Name}", true);
                File.Copy(processo.ArquivoHouse, $"{directoryProcessed}\\{diretorioProcesso}\\{new FileInfo(processo.ArquivoHouse).Name}", true);
                File.Copy(processo.ArquivoNF, $"{directoryProcessed}\\{diretorioProcesso}\\{new FileInfo(processo.ArquivoNF).Name}", true);

                File.Delete(processo.ArquivoHouse);
                File.Delete(processo.ArquivoFatura);
                File.Delete(processo.ArquivoNF);
            }
        }

        private void MoverArquivosNaoProcessados(List<FileInfo> files)
        {
            if (!Directory.Exists($"{_roboRobinsonSettings.diretorioArquivosErros}\\{diretorioProcesso}")) Directory.CreateDirectory($"{directoryProcessed}\\{diretorioProcesso}");

            foreach (var file in files)
            {
                File.Copy(file.FullName, $"{directoryProcessed}\\{diretorioProcesso}\\{file.Name}", true);
                File.Delete(file.FullName);
            }
        }
        private void LogInformation(string menssage)
        {
            _logger.LogInformation($"{nameof(FindProcessoService)} - {menssage} --- hora: { DateTime.Now}");

        }
        private void LogError(string menssage)
        {
            _logger.LogError($"{nameof(FindProcessoService)} - {menssage} --- hora: { DateTime.Now}");

        }

    }
}

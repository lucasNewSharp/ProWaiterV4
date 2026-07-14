using IWshRuntimeLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace CustomActionsSetup
{
    [RunInstaller(true)]
    public partial class ConfiguracoesAposInstalacao : Installer
    {
        private string _diretorioIconeStartMenu;
        private string _diretorioDesktop;
        private string _diretorioInicializar;
        private const string _nomeLinkProWaiter = "ProWaiter.lnk";
        private const string _nomeLinkValidadorLicenca = "ProWaiterLicenca.lnk";
        private const string _nomeLinkICBox = "ProWaiterICBox.lnk";

        public ConfiguracoesAposInstalacao()
        {
            InitializeComponent();
            _diretorioIconeStartMenu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "NewSharp");
            _diretorioDesktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
            _diretorioInicializar = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(@"C:\inetpub\wwwroot\ProWaiter");
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                FileSystemAccessRule iis_iusrs = new FileSystemAccessRule("IIS_IUSRS",
                                                                           FileSystemRights.Modify,
                                                                           InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                                                           PropagationFlags.None,
                                                                           AccessControlType.Allow);

                dSecurity.AddAccessRule(iis_iusrs);
                dInfo.SetAccessControl(dSecurity);

                Directory.CreateDirectory(_diretorioIconeStartMenu);
                //Criar atalho
                CriarAtalhoProWaiter(_diretorioDesktop);
                CriarAtalhoProWaiter(_diretorioIconeStartMenu);

                CriarAtalhoValidadorLicenca(_diretorioInicializar);
                CriarAtalhoValidadorLicenca(_diretorioIconeStartMenu);

                CriarAtalhoICBox(_diretorioInicializar);
                CriarAtalhoICBox(_diretorioIconeStartMenu);

                Process.Start(Path.Combine(_diretorioInicializar, _nomeLinkICBox));
                Process.Start(Path.Combine(_diretorioInicializar, _nomeLinkValidadorLicenca));
            }
            catch(Exception ex)
            {
                //LOG
                throw ex;
            }
        }

        private void CriarAtalhoProWaiter(string caminhoAtalho)
        {
            //Criar atalho
            WshShell shell = new WshShell();

            caminhoAtalho = Path.Combine(caminhoAtalho, _nomeLinkProWaiter);
            IWshShortcut atalho = (IWshShortcut)shell.CreateShortcut(caminhoAtalho);
            atalho.Description = "Atalho para o ProWaiter";
            atalho.TargetPath = "http://localhost/ProWaiter/";
            atalho.IconLocation = @"c:\inetpub\wwwroot\prowaiter\ProWaiter.ico";
            atalho.Save();
        }

        private void CriarAtalhoValidadorLicenca(string caminhoAtalho)
        {
            //Criar atalho
            WshShell shell = new WshShell();

            caminhoAtalho = Path.Combine(caminhoAtalho, _nomeLinkValidadorLicenca);
            IWshShortcut atalho = (IWshShortcut)shell.CreateShortcut(caminhoAtalho);
            atalho.Description = "Atalho para o ProWaiter - Licenca";
            atalho.TargetPath = @"c:\inetpub\wwwroot\prowaiter\ValidadorLicenca\ProWaiter.Licenca.exe";
            atalho.IconLocation = @"c:\inetpub\wwwroot\prowaiter\ValidadorLicenca\Imagens\ProWaiterLicenca.ico";
            atalho.Save();
        }

        private void CriarAtalhoICBox(string caminhoAtalho)
        {
            //Criar atalho
            WshShell shell = new WshShell();

            caminhoAtalho = Path.Combine(caminhoAtalho, _nomeLinkICBox);
            IWshShortcut atalho = (IWshShortcut)shell.CreateShortcut(caminhoAtalho);
            atalho.Description = "Atalho para o ProWaiter - ICBox";
            atalho.TargetPath = @"c:\inetpub\wwwroot\prowaiter\ICBox\ProWaiter.ICBox.exe";
            atalho.IconLocation = @"c:\inetpub\wwwroot\prowaiter\ICBox\Icone\ProWaiter_22x22.ico";
            atalho.Save();
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);
            System.IO.File.Delete(Path.Combine(_diretorioDesktop, _nomeLinkProWaiter));
            System.IO.File.Delete(Path.Combine(_diretorioIconeStartMenu, _nomeLinkProWaiter));

            System.IO.File.Delete(Path.Combine(_diretorioInicializar, _nomeLinkValidadorLicenca));
            System.IO.File.Delete(Path.Combine(_diretorioIconeStartMenu, _nomeLinkValidadorLicenca));

            System.IO.File.Delete(Path.Combine(_diretorioInicializar, _nomeLinkICBox));
            System.IO.File.Delete(Path.Combine(_diretorioIconeStartMenu, _nomeLinkICBox));
        }
    }
}
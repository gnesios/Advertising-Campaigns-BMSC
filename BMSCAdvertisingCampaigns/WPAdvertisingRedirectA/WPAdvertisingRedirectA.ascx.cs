using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace BMSCAdvertisingCampaigns.WPAdvertisingRedirectA
{
    [ToolboxItemAttribute(false)]
    public partial class WPAdvertisingRedirectA : WebPart
    {
        const string ADVERTISING_PARAMETERS = "Publicidad Parámetros";
        const string ADVERTISING_RECORDS = "Publicidad Registros";

        #region Web part parameters
        /*private int spAdvertisingId;
        [Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("ID campaña"),
        WebDescription("Identificador de la campaña a ser usada en el formulario."),
        Category("Configuración")]
        public int SpAdvertisingId
        {
            get { return spAdvertisingId; }
            set { spAdvertisingId = value; }
        }*/

        public enum Templates { Plantilla_A, Plantilla_B };
        private Templates spAdvertisingTemplate;
        [Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Plantilla"),
        WebDescription("Plantilla usada en el formulario."),
        Category("Configuración")]
        public Templates SpAdvertisingTemplate
        {
            get { return spAdvertisingTemplate; }
            set { spAdvertisingTemplate = value; }
        }
        #endregion

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public WPAdvertisingRedirectA()
        {
            //SpAdvertisingId = 0;
            SpAdvertisingTemplate = Templates.Plantilla_A;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetResponceData();
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl deadlineMessage = new System.Web.UI.LiteralControl();
                deadlineMessage.Text =
                    "<link rel='stylesheet' href='/_catalogs/masterpage/bmsc/styles/advertising_thanks_a.css'>" +
                    "<div class='advertisingMessage'>" + ex.Message + "</div>";

                this.Controls.Clear();
                this.Controls.Add(deadlineMessage);
            }
        }

        /// <summary>
        /// Obtiene toda la informacion para ser mostrada en la pagina.
        /// </summary>
        private void GetResponceData()
        {
            if (SpAdvertisingTemplate == Templates.Plantilla_A)
            {
                ltrStyle.Text = "<link rel='stylesheet' href='/_catalogs/masterpage/bmsc/styles/advertising_thanks_a.css'>";
            }
            else
            {
                ltrStyle.Text = "<link rel='stylesheet' href='/_catalogs/masterpage/bmsc/styles/advertising_thanks_b.css'>";
            }

            string advertisingTitle = "";
            string advertisingDescription = "";
            string advertisingImage = "";
            string advertisingAttach = "";
            bool advertisingDownload = false;

            string advertisingId = "";
            string advertisingRecordId = "";
            string advertisingRecordClient = "";
            string advertisingRecordOffice = "";

            using (SPSite sps = new SPSite(SPContext.Current.Web.Url))
            using (SPWeb spw = sps.OpenWeb())
            {
                /*Consulta para obtener datos de la campaña*/
                advertisingId = Page.Request.QueryString["ad"];
                SPQuery queryAdv = new SPQuery();
                queryAdv.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='ID' /><Value Type='Counter'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    advertisingId);
                SPListItemCollection advertising = spw.Lists[ADVERTISING_PARAMETERS].GetItems(queryAdv);

                if (advertising.Count == 1)
                {
                    SPListItem theAdvertising = advertising[0];

                    advertisingTitle = theAdvertising["T_x00ed_tulo_x0020_Agradecimient"].ToString();
                    advertisingDescription = theAdvertising["Descripci_x00f3_n_x0020_Agradeci"].ToString().Replace("\n", "<br/>");
                    advertisingDownload = Convert.ToBoolean(theAdvertising["Descargar_x0020_Campa_x00f1_a"].ToString());
                    advertisingImage = theAdvertising["Imagen_x0020_Agradecimiento"].ToString();
                    if (advertisingImage.Contains(","))
                        advertisingImage = advertisingImage.Remove(advertisingImage.IndexOf(','));
                    if (theAdvertising.Attachments.Count > 0)
                    {
                        advertisingAttach = string.Format(
                            "/Lists/Publicidad%20Parmetros/Attachments/{0}/{1}",
                            theAdvertising.ID, theAdvertising.Attachments[0]);
                    }
                }
                else
                {
                    System.Web.UI.LiteralControl deadlineMessage = new System.Web.UI.LiteralControl();
                    deadlineMessage.Text =
                        "<link rel='stylesheet' href='/_catalogs/masterpage/bmsc/styles/advertising_thanks_a.css'>" +
                        "<div class='advertisingMessage'>" +
                        "La campaña con el ID <b>" + advertisingId + "</b> no existe o no esta aprobada.</div>";

                    this.Controls.Clear();
                    this.Controls.Add(deadlineMessage);
                }

                /*Consulta para obtener datos del cliente*/
                advertisingRecordId = Page.Request.QueryString["cl"];
                SPListItem theClient = spw.Lists[ADVERTISING_RECORDS].GetItemById(Convert.ToInt32(advertisingRecordId));
                advertisingRecordClient = theClient["Title"].ToString().Trim().ToUpper();
                advertisingRecordOffice = theClient["Agencia_x0020_Registro"].ToString().Substring(
                    theClient["Agencia_x0020_Registro"].ToString().LastIndexOf('|') + 1).ToUpper().Trim();
            }

            lblAdvertisingName.Text = advertisingTitle.Replace("[C]", advertisingRecordClient).Replace("[A]", advertisingRecordOffice);
            lblAdvertisingDescription.Text = advertisingDescription.Replace("[C]", advertisingRecordClient).Replace("[A]", advertisingRecordOffice);
            ltrAdvertisingDownload.Text = this.FormatDownloadControl(advertisingDownload, advertisingAttach);
            if (SpAdvertisingTemplate != Templates.Plantilla_A)
            {
                ltrBackgroundB.Text = string.Format(
                    "<style type='text/css'>" +
                    "html {{" +
                    "height:100%; min-height:100%;" +
                    "background: url('{0}') no-repeat center center fixed;" +
                    "-webkit-background-size: cover;" +
                    "-moz-background-size: cover;" +
                    "-o-background-size: cover;" +
                    "background-size: cover;" +
                    "}}" +
                    "</style>",
                    advertisingImage);
            }
        }

        /// <summary>
        /// Formatea el control para la opcion de 'descarga'.
        /// </summary>
        /// <param name="advertisingDownload"></param>
        /// <param name="advertisingAttach"></param>
        /// <returns></returns>
        private string FormatDownloadControl(bool advertisingDownload, string advertisingAttach)
        {
            string formatedString = "";

            if (advertisingDownload && advertisingAttach != "")
            {
                formatedString = string.Format(
                    "<a href='{0}'>Descargar</a>",
                    advertisingAttach);
            }

            return formatedString;
        }
    }
}

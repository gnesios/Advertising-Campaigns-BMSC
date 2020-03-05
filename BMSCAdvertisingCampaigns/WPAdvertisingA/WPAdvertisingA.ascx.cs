using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace BMSCAdvertisingCampaigns.WPAdvertisingA
{
    [ToolboxItemAttribute(false)]
    public partial class WPAdvertisingA : WebPart
    {
        const string ADVERTISING_RECORDS = "Publicidad Registros";
        const string ADVERTISING_PARAMETERS = "Publicidad Parámetros";
        const string ADVERTISING_OFFICES = "Red de Oficinas";

        List<Office> advertisingOffices;
        List<string> advertisingCities;

        #region Web part parameters
        private int spAdvertisingId;
        [Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("ID campaña"),
        WebDescription("Identificador de la campaña a ser usada en el formulario."),
        Category("Configuración")]
        public int SpAdvertisingId
        {
            get { return spAdvertisingId; }
            set { spAdvertisingId = value; }
        }

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

        private string spAdvertisingRedirect;
        [Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Página de redirección"),
        WebDescription("Nombre de la página de redirección."),
        Category("Configuración")]
        public string SpAdvertisingRedirect
        {
            get { return spAdvertisingRedirect; }
            set { spAdvertisingRedirect = value; }
        }
        #endregion

        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public WPAdvertisingA()
        {
            SpAdvertisingId = 0;
            SpAdvertisingTemplate = Templates.Plantilla_A;
            SpAdvertisingRedirect = "";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();

            try
            {
                this.GetFormData();
            }
            catch (Exception ex)
            {
                System.Web.UI.LiteralControl deadlineMessage = new System.Web.UI.LiteralControl();
                deadlineMessage.Text =
                    "<link rel='stylesheet' href='/_catalogs/masterpage/bmsc/styles/advertising_a.css'>" +
                    "<div class='advertisingMessage'>" + ex.Message + "</div>";

                this.Controls.Clear();
                this.Controls.Add(deadlineMessage);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    string newItemId = "";

                    using (SPSite sps = new SPSite(SPContext.Current.Web.Url))
                    using (SPWeb spw = sps.OpenWeb())
                    {
                        SPListItemCollection itemsAdvertisingRecords = spw.Lists[ADVERTISING_RECORDS].Items;
                        SPListItem newItem = itemsAdvertisingRecords.Add();

                        List<string> sendMailResponce = new List<string>(2);

                        System.Web.UI.WebControls.DropDownList officeElement =
                            new System.Web.UI.WebControls.DropDownList();
                        string nameBox;
                        string phoneBox;
                        if (SpAdvertisingTemplate == Templates.Plantilla_A)
                        {
                            officeElement = ddlOfficeA;
                            nameBox = txbNameA.Text;
                            phoneBox = txbPhoneA.Text;
                        }
                        else
                        {
                            officeElement = ddlOfficeB;
                            nameBox = txbNameB.Text;
                            phoneBox = txbPhoneB.Text;
                        }
                        
                        sendMailResponce = this.SendMail(
                            spw, officeElement.SelectedIndex - 1, nameBox, phoneBox);

                        newItem["Title"] = nameBox;
                        newItem["T_x00ed_tulo_x0020_Campa_x00f1_a"] = spw.Lists[ADVERTISING_PARAMETERS].GetItemById(SpAdvertisingId).Title;
                        newItem["Tel_x00e9_fono_x0020_Registro"] = phoneBox;
                        newItem["Agencia_x0020_Registro"] = officeElement.SelectedItem.Value + " | " + officeElement.SelectedItem.Text; 
                        newItem["Correo_x0020_Enviado"] = Convert.ToBoolean(sendMailResponce[0]);
                        newItem["Comentarios_x0020_Registro"] = sendMailResponce[1];

                        newItem.Update();
                        newItemId = newItem.ID.ToString();
                    }

                    Page.Response.Redirect(SpAdvertisingRedirect + "?ad=" + SpAdvertisingId + "&cl=" + newItemId, true);
                }
            }
            catch (Exception ex)
            {
                //pnlFormA.Visible = false;
                //pnlFormB.Visible = false;
                System.Web.UI.LiteralControl deadlineMessage = new System.Web.UI.LiteralControl();
                deadlineMessage.Text =
                    "<link rel='stylesheet' href='/_catalogs/masterpage/bmsc/styles/advertising_a.css'>" +
                    "<div class='advertisingMessage'>" + ex.Message + "</div>";

                this.Controls.Clear();
                this.Controls.Add(deadlineMessage);
            }
        }

        /// <summary>
        /// Envia correos electronicos a usuarios registrados en agencias.
        /// </summary>
        /// <param name="spw"></param>
        /// <param name="index"></param>
        /// <param name="contactName"></param>
        /// <param name="contactPhone"></param>
        /// <returns></returns>
        private List<string> SendMail(SPWeb spw, int index, string contactName, string contactPhone)
        {
            List<string> sendMailResponce = new List<string>(2);
            string advertisingName = spw.Lists[ADVERTISING_PARAMETERS].GetItemById(SpAdvertisingId).Title;
            Office selectedOffice = advertisingOffices[index];

            #region Notification mails
            if (!string.IsNullOrWhiteSpace(selectedOffice.Mail))
            {
                string subject = "Cliente interesado en campaña '" + advertisingName + "'";
                string body = string.Format(
                    "Estimad@: El cliente <b>{0}<b/> a través de nuestra página web ha declarado estar interesado en la campaña " +
                    "<b>{1}<b/>, su número de contacto es el {2}.<br/><br/>" +
                    "Es importante ponerse en contacto con el cliente en un periodo máximo de 48 horas de registrado el mail para cumplir con nuestra promesa de contacto. Se realizará un control posterior para verificar el contacto:"+
                    "<br/>Equipo de Marketing del Banco Mercantil Santa Cruz.",
                    contactName, advertisingName, contactPhone);

                System.Collections.Specialized.StringDictionary headers =
                    new System.Collections.Specialized.StringDictionary();
                headers.Add("to", selectedOffice.Mail);
                headers.Add("cc", "");
                headers.Add("bcc", "");
                headers.Add("from", "marketing@bmsc.com.bo");
                headers.Add("subject", subject);
                headers.Add("content-type", "text/html");
                string bodyText = string.Format(
                    "<table border='0' cellspacing='0' cellpadding='0' style='width:99%;border-collapse:collapse;'>" +
                    "<tr><td style='border:solid #E8EAEC 1.0pt;background:#F8F8F9;padding:12.0pt 7.5pt 15.0pt 7.5pt'>" +
                    "<p style='font-size:15.0pt;font-family:Verdana,sans-serif;'>" +
                    "Campañas BMSC: Notificación de nuevo registro para \"{0}\"</p></td></tr>" +
                    "<tr><td style='border:none;border-bottom:solid #9CA3AD 1.0pt;padding:4.0pt 7.5pt 4.0pt 7.5pt'>" +
                    "<p style='font-size:10.0pt;font-family:Tahoma,sans-serif'>" +
                    "{1}" +
                    "</p></td></tr></table>",
                    advertisingName, body);

                try
                {
                    Microsoft.SharePoint.Utilities.SPUtility.SendEmail(spw, headers, bodyText);
                    
                    sendMailResponce.Add("true");
                    sendMailResponce.Add("Correos enviados exitosamente a las direcciones: " + selectedOffice.Mail);
                }
                catch (Exception ex)
                {
                    sendMailResponce.Add("false");
                    sendMailResponce.Add("ERROR >> " + ex.Message);
                }
            }
            else
            {
                sendMailResponce.Add("false");
                sendMailResponce.Add("ERROR >> No existen correos asociados a la agencia indicada.");
            }
            #endregion

            return sendMailResponce;
        }

        /// <summary>
        /// Obtiene los datos de Ciudad y Agencia para el formulario.
        /// </summary>
        private void GetFormData()
        {
            if (SpAdvertisingTemplate == Templates.Plantilla_A)
            {
                ltrStyle.Text = "<link rel='stylesheet' href='/_catalogs/masterpage/bmsc/styles/advertising_a.css'>";
                pnlFormA.Visible = true;
            }
            else
            {
                ltrStyle.Text = "<link rel='stylesheet' href='/_catalogs/masterpage/bmsc/styles/advertising_b.css'>";
                pnlFormB.Visible = true;
            }

            advertisingOffices = new List<Office>();
            advertisingCities = new List<string>();
            string advertisingTitle = "";
            string advertisingDescription = "";
            string advertisingImage = "";
            string advertisingNote = "";
            string advertisingDeadline = "";
            string advertisingButton = "";
            bool advertisingCountdown = false;

            using (SPSite sps = new SPSite(SPContext.Current.Web.Url))
            using (SPWeb spw = sps.OpenWeb())
            {
                /*Consulta para obtener Ciudades y demas datos del formulario*/
                SPQuery queryAdv = new SPQuery();
                queryAdv.Query = string.Format(
                    "<Where><And>" +
                    "<Eq><FieldRef Name='ID' /><Value Type='Counter'>{0}</Value></Eq>" +
                    "<Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "</And></Where>",
                    SpAdvertisingId);
                SPListItemCollection advertising = spw.Lists[ADVERTISING_PARAMETERS].GetItems(queryAdv);

                if (advertising.Count == 1)
                {
                    SPListItem theAdvertising = advertising[0];

                    string[] stringSeparators = new string[] { ";#" };
                    string cities = theAdvertising["Ciudades_x0020_Campa_x00f1_a"].ToString().ToUpper();
                    advertisingCities = new List<string>(cities.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries));

                    foreach (string city in advertisingCities)
                    {
                        /*Consulta para obtener Agencias, en base a las Ciudades elegidas*/
                        SPQuery queryOffice = new SPQuery();
                        queryOffice.Query = string.Format(
                            "<OrderBy><FieldRef Name='Departamento0' /><FieldRef Name='Title' /></OrderBy>" +
                            "<Where><And>" +
                            "<Eq><FieldRef Name='Departamento0' /><Value Type='Text'>{0}</Value></Eq>" +
                            "<Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                            "</And></Where>",
                            city);
                        SPListItemCollection offices = spw.Lists[ADVERTISING_OFFICES].GetItems(queryOffice);

                        foreach (SPListItem office in offices)
                        {
                            //string officeMail = office.Title.Replace(" ", "") + "@gnet.com";
                            string officeMail = "";
                            if (office["Correo_x0020_oficina"] != null)
                                officeMail = office["Correo_x0020_oficina"].ToString().Trim();

                            advertisingOffices.Add(new Office(office.ID, office.Title.Trim(),
                                office["Departamento0"].ToString().Trim(), office["Direcci_x00f3_n"].ToString().Trim(),
                                officeMail));
                        }
                    }

                    /*Recuperar los demas datos para el formulario*/
                    advertisingTitle = theAdvertising.Title;
                    advertisingDescription = theAdvertising["Descripci_x00f3_n_x0020_Campa_x0"].ToString().Replace("\n", "<br/>");
                    advertisingImage = theAdvertising["Imagen_x0020_Campa_x00f1_a"].ToString();
                    if (advertisingImage.Contains(","))
                        advertisingImage = advertisingImage.Remove(advertisingImage.IndexOf(','));
                    if (theAdvertising["Nota_x0020_Campa_x00f1_a"] != null)
                        advertisingNote = theAdvertising["Nota_x0020_Campa_x00f1_a"].ToString();
                    if (theAdvertising["Vigencia_x0020_Campa_x00f1_a"] != null)
                        advertisingDeadline = theAdvertising["Vigencia_x0020_Campa_x00f1_a"].ToString();
                    advertisingButton = theAdvertising["Bot_x00f3_n_x0020_Campa_x00f1_a"].ToString();
                    advertisingCountdown = Convert.ToBoolean(theAdvertising["Cuenta_x0020_Regresiva"].ToString());
                }
                else
                {
                    //pnlFormA.Visible = false;
                    //pnlFormB.Visible = false;
                    System.Web.UI.LiteralControl deadlineMessage = new System.Web.UI.LiteralControl();
                    deadlineMessage.Text =
                        "<link rel='stylesheet' href='/_catalogs/masterpage/bmsc/styles/advertising_a.css'>" +
                        "<div class='advertisingMessage'>" +
                        "La campaña con el ID <b>" + SpAdvertisingId + "</b> no existe o no esta aprobada.</div>";

                    this.Controls.Clear();
                    this.Controls.Add(deadlineMessage);
                }
            }

            lblAdvertisingNameA.Text = advertisingTitle;
            lblAdvertisingNameB.Text = advertisingTitle;
            lblAdvertisingDescriptionA.Text = advertisingDescription;
            lblAdvertisingDescriptionB.Text = advertisingDescription;
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
            lblAdvertisingNoteA.Text = advertisingNote;
            lblAdvertisingNoteB.Text = advertisingNote;
            btnSendA.Text = advertisingButton;
            btnSendB.Text = advertisingButton;

            ddlCityA.DataSource = advertisingCities;
            ddlCityB.DataSource = advertisingCities;
            ddlCityA.DataBind();
            ddlCityB.DataBind();
            ddlCityA.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", string.Empty));
            ddlCityB.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", string.Empty));

            ddlOfficeA.DataValueField = "Id";
            ddlOfficeB.DataValueField = "Id";
            ddlOfficeA.DataTextField = "Display";
            ddlOfficeB.DataTextField = "Display";
            ddlOfficeA.DataSource = advertisingOffices;
            ddlOfficeB.DataSource = advertisingOffices;
            ddlOfficeA.DataBind();
            ddlOfficeB.DataBind();
            ddlOfficeA.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", string.Empty));
            ddlOfficeB.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", string.Empty));

            ltrCountdownA.Text = this.FormatCountdownControl(advertisingCountdown, advertisingDeadline);
            ltrCountdownB.Text = this.FormatCountdownControl(advertisingCountdown, advertisingDeadline);
        }

        /// <summary>
        /// Formatea el control para la opcion de 'cuenta regresiva'.
        /// </summary>
        /// <param name="advertisingCountdown"></param>
        /// <param name="advertisingDeadline"></param>
        /// <returns></returns>
        private string FormatCountdownControl(bool advertisingCountdown, string advertisingDeadline)
        {
            string formatedString = "";

            if (advertisingDeadline != "")
            {
                DateTime deadlineDate = Convert.ToDateTime(advertisingDeadline);

                if (deadlineDate > DateTime.Now)
                {
                    if (advertisingCountdown)
                    {
                        formatedString = string.Format(
                            "<script type='text/template' id='countdown-zone-template'>" +
                            "<div class='time <%= label %>'>" +
                            "<span class='count curr top'><%= curr %></span>" +
                            "<span class='count next top'><%= next %></span>" +
                            "<span class='count next bottom'><%= next %></span>" +
                            "<span class='count curr bottom'><%= curr %></span>" +
                            "<span class='label'><%= label %></span>" +
                            "</div></script>" +
                            "<script type='text/javascript'>" +
                            /*INICIO - filtro de ciudades y agencias*/
                            "$('div.city select').change(function () {{" +
                            "if ($(this).data('options') === undefined) {{" +
                            "$(this).data('options', $('div.office select option').clone()); }}" +
                            "var city = $(this).val();" +
                            "var options = $(this).data('options').filter(':contains(\"' + city + '\")');" +
                            "$('div.office select').html(options); }});" +
                            /*FIN - filtro de ciudades y agencias*/
                            "$(window).on('load', function() {{" +
                            "var labels = ['días', 'horas', 'mins', 'segs']," +
                            "deadline = '{0}'," +
                            "template = _.template($('#countdown-zone-template').html())," +
                            "currDate = '00:00:00:00'," +
                            "nextDate = '00:00:00:00'," +
                            "parser = /([0-9]{{2,}})/gi," +
                            "$example = $('{1}');" +
                            "function strfobj(str) {{" +
                            "var parsed = str.match(parser), obj = {{}};" +
                            "labels.forEach(function(label, i) {{ obj[label] = parsed[i] }});" +
                            "return obj; }}" +
                            "function diff(obj1, obj2) {{" +
                            "var diff = [];" +
                            "labels.forEach(function(key) {{ if (obj1[key] !== obj2[key]) {{ diff.push(key); }} }});" +
                            "return diff; }}" +
                            "var initData = strfobj(currDate);" +
                            "labels.forEach(function(label, i) {{" +
                            "$example.append(template({{ curr: initData[label], next: initData[label], label: label }})); }});" +
                            "$example.countdown(deadline, function(event) {{" +
                            "var newDate = event.strftime('%D:%H:%M:%S'), data;" +
                            "if (newDate !== nextDate) {{" +
                            "currDate = nextDate;" +
                            "nextDate = newDate;" +
                            "data = {{ 'curr': strfobj(currDate), 'next': strfobj(nextDate) }};" +
                            "diff(data.curr, data.next).forEach(function(label) {{" +
                            "var selector = '.%s'.replace(/%s/, label), $node = $example.find(selector);" +
                            "$node.removeClass('flip');" +
                            "$node.find('.curr').text(data.curr[label]);" +
                            "$node.find('.next').text(data.next[label]);" +
                            "_.delay(function($node) {{ $node.addClass('flip'); }}, 50, $node);" +
                            "}}); }} }}); }});" +
                            "</script>",
                            deadlineDate.ToString("yyyy/M/d HH:mm"),
                            SpAdvertisingTemplate == Templates.Plantilla_A ? "#countdown-zone-a" : "#countdown-zone-b");
                    }
                }
                else
                {
                    //pnlFormA.Visible = false;
                    //pnlFormB.Visible = false;
                    string deadlineString =
                        "<style type='text/css'>.advertisingform { display: none !important; }</style>" +
                        "<p>Lo sentimos, la campaña ya no se encuentra vigente.<br/>Quédate atento a las próximas campañas, estamos seguros serán de tu interés.</p>";
                    lblDeadlineMessageA.Text = deadlineString;
                    lblDeadlineMessageB.Text =
                        "<div class='deadlineheader'><span>Formulario de Registro</span></div>" +
                        deadlineString;
                    lblDeadlineMessageA.Visible = true;
                }
            }

            return formatedString;
        }
    }

    public class Office
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string city;
        public string City
        {
            get { return city; }
            set { city = value; }
        }
        private string address;
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        private string mail;
        public string Mail
        {
            get { return mail; }
            set { mail = value; }
        }
        private string display;
        public string Display
        {
            get { return display; }
            set { display = value; }
        }

        public Office()
        {
            this.Id = 0;
            this.Name = "";
            this.City = "";
            this.Address = "";
            this.Mail = "";
            this.Display = "";
        }

        public Office(int id, string name, string city, string address, string mail)
        {
            this.Id = id;
            this.Name = name;
            this.City = city.ToUpper();
            this.Address = address;
            this.Mail = mail;
            this.Display = this.City + " | " + this.Name;
        }
    }
}

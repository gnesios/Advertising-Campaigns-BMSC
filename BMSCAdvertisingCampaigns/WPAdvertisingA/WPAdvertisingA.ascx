<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WPAdvertisingA.ascx.cs" Inherits="BMSCAdvertisingCampaigns.WPAdvertisingA.WPAdvertisingA" %>

<asp:Literal runat="server" ID="ltrStyle"></asp:Literal>

<asp:Panel runat="server" ID="pnlFormA" Visible="false">
<div class="background_element"></div>
<div class="container">
    <div class="title"><asp:Label runat="server" ID="lblAdvertisingNameA"></asp:Label></div>
    <div class="containerside">
        <div class="leftside">
            <div class="description">
                <asp:Label runat="server" ID="lblAdvertisingDescriptionA"></asp:Label>
                <asp:Label runat="server" ID="lblAdvertisingNoteA" CssClass="note"></asp:Label>
            </div>
        </div>
        <div class="rightside">
            <div class="logo"></div>
            <asp:Label runat="server" ID="lblDeadlineMessageA" CssClass="deadlineMessage" Visible="false"></asp:Label>
            <div class="advertisingform">
                <label for="fldName">
                    <span>Nombre </span>
                    <asp:RequiredFieldValidator ID="rfvNameA" runat="server" Text="(requerido)" CssClass="required"
                        Display="Dynamic" ControlToValidate="txbNameA" SetFocusOnError="true" />
                    <div class="name">
                        <asp:TextBox runat="server" ID="txbNameA" MaxLength="32" ToolTip="Ingresa tu nombre" />
                    </div>
                </label>
                <label for="fldPhone">
                    <span>Teléfono </span>
                    <asp:RequiredFieldValidator ID="rfvPhoneA" runat="server" Text="(requerido)" CssClass="required"
                        Display="Dynamic" ControlToValidate="txbPhoneA" SetFocusOnError="true" />
                    <asp:RegularExpressionValidator ID="rxvPhoneA" runat="server" Text="(no válido)" CssClass="required"
                        Display="Dynamic" ControlToValidate="txbPhoneA" ValidationExpression="\d+" SetFocusOnError="true" />
                    <div class="phone">
                        <asp:TextBox runat="server" ID="txbPhoneA" MaxLength="8" ToolTip="Ingresa tu teléfono" />
                    </div>
                </label>
                <label for="fldCity">
                    <span>Departamento</span>
                    <asp:RequiredFieldValidator runat="server" ID="rfvCityA" Text="(requerido)" CssClass="required"
                        InitialValue="" Display="Dynamic" ControlToValidate="ddlCityA" SetFocusOnError="true" />
                    <div class="city">
                        <asp:DropDownList runat="server" ID="ddlCityA" ToolTip="Elige tu departamento"></asp:DropDownList>
                    </div>
                </label>
                <label for="fldOffice">
                    <span>Agencia más cercana </span>
                    <asp:RequiredFieldValidator runat="server" ID="rfvOfficeA" Text="(requerido)" CssClass="required"
                        InitialValue="" Display="Dynamic" ControlToValidate="ddlOfficeA" SetFocusOnError="true" />
                    <div class="office">
                        <asp:DropDownList runat="server" ID="ddlOfficeA" ToolTip="Elige tu agencia"></asp:DropDownList>
                    </div>
                </label>
                <asp:Button runat="server" ID="btnSendA" Text="" OnClick="btnSend_Click" />
                <div id="clock-a" class="countdown-text"></div>
                <div class="countdown-zone">
                    <div class="countdown-container" id="countdown-zone-a"></div>
                    <div class="countdown-note">tiempo restante para la finalización de esta campaña</div>
                </div>
                <asp:Literal runat="server" ID="ltrCountdownA"></asp:Literal>
            </div>
        </div>
    </div>
</div>
</asp:Panel>

<asp:Panel runat="server" ID="pnlFormB" Visible="false">
<asp:Literal runat="server" ID="ltrBackgroundB"></asp:Literal>
<div class="background_element"></div>
<div class="container">
    <div class="containerside">
        <div class="leftside">
            <div class="logo"></div>
            <div class="title"><asp:Label runat="server" ID="lblAdvertisingNameB"></asp:Label></div>
            <div class="description">
                <asp:Label runat="server" ID="lblAdvertisingDescriptionB"></asp:Label>
                <asp:Label runat="server" ID="lblAdvertisingNoteB" CssClass="note"></asp:Label>
            </div>
        </div>
        <div class="rightside">
            <asp:Label runat="server" ID="lblDeadlineMessageB" CssClass="deadlineMessage"></asp:Label>
            <div class="advertisingform">
                <div class="header"><span>Formulario de Registro</span></div>
                <label for="fldName">
                    <span>Nombre </span>
                    <asp:RequiredFieldValidator ID="rfvNameB" runat="server" Text="(requerido)" CssClass="required"
                        Display="Dynamic" ControlToValidate="txbNameB" SetFocusOnError="true" />
                    <div class="name">
                        <asp:TextBox runat="server" ID="txbNameB" MaxLength="32" ToolTip="Ingresa tu nombre" />
                    </div>
                </label>
                <label for="fldPhone">
                    <span>Teléfono </span>
                    <asp:RequiredFieldValidator ID="rfvPhoneB" runat="server" Text="(requerido)" CssClass="required"
                        Display="Dynamic" ControlToValidate="txbPhoneB" SetFocusOnError="true" />
                    <asp:RegularExpressionValidator ID="rxvPhoneB" runat="server" Text="(no válido)" CssClass="required"
                        Display="Dynamic" ControlToValidate="txbPhoneB" ValidationExpression="\d+" SetFocusOnError="true" />
                    <div class="phone">
                        <asp:TextBox runat="server" ID="txbPhoneB" MaxLength="8" ToolTip="Ingresa tu teléfono" />
                    </div>
                </label>
                <label for="fldCity">
                    <span>Departamento</span>
                    <asp:RequiredFieldValidator runat="server" ID="rfvCityB" Text="(requerido)" CssClass="required"
                        InitialValue="" Display="Dynamic" ControlToValidate="ddlCityB" SetFocusOnError="true" />
                    <div class="city">
                        <asp:DropDownList runat="server" ID="ddlCityB" ToolTip="Elige tu departamento"></asp:DropDownList>
                    </div>
                </label>
                <label for="fldOffice">
                    <span>Agencia más cercana </span>
                    <asp:RequiredFieldValidator runat="server" ID="rfvOfficeB" Text="(requerido)" CssClass="required"
                        InitialValue="" Display="Dynamic" ControlToValidate="ddlOfficeB" SetFocusOnError="true" />
                    <div class="office">
                        <asp:DropDownList runat="server" ID="ddlOfficeB" ToolTip="Elige tu agencia"></asp:DropDownList>
                    </div>
                </label>
                <asp:Button runat="server" ID="btnSendB" Text="" OnClick="btnSend_Click" />
                <div id="clock-b" class="countdown-text"></div>
                <div class="countdown-zone">
                    <div class="countdown-container" id="countdown-zone-b"></div>
                    <div class="countdown-note">tiempo restante para la finalización de esta campaña</div>
                </div>
                <asp:Literal runat="server" ID="ltrCountdownB"></asp:Literal>
            </div>
        </div>
    </div>
</div>
</asp:Panel>
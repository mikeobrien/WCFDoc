<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >

	<xsl:output method="html" />

	<xsl:template match="/doc">

		<html>
		<head>
			<Title>Web Service API</Title>
			<style>
			body {
				font-family: Verdana, Arial;
				font-size: 8.5pt;	
			}
			h2 {
				font-family: Verdana, Arial;
				font-size: 18pt;
			}
			h3 {
				font-family: Verdana, Arial;
				font-size: 16pt;
			}
			h4 {
				font-family: Verdana, Arial;
				font-size: 12pt;
			}
			h5 {
				font-family: Verdana, Arial;
				font-size: 8.5pt;
			}
			table {
				border-style: solid;
				border-top-width: 1px;
				border-left-width: 1px;
				border-bottom-width: 0px;
				border-right-width: 0px;
				border-color: #CCCCCC;
				border-collapse: collapse;
			}
			th {
				font-family: Verdana, Arial;
				font-size: 8.5pt;	
				background: #CCCCCC;
				border-style: solid;
				border-top-width: 0px;
				border-left-width: 0px;
				border-bottom-width: 1px;
				border-right-width: 1px;
				border-color: #CCCCCC;
			}
			td {
				font-family: Verdana, Arial;
				font-size: 8.5pt;	
				border-style: solid;
				border-top-width: 0px;
				border-left-width: 0px;
				border-bottom-width: 1px;
				border-right-width: 1px;
				border-color: #CCCCCC;
			}
			hr {
				margin-top: 3;
				margin-bottom: 3;
				display: block;
			}
			.SmallMargin {
				margin-top: 3;
				margin-bottom: 3;
			}
			</style>
		</head>
		<body topmargin="40" leftmargin="40" rightmargin="40" bottommargin="40">
		<h2>Web Service API</h2>

		<xsl:if test="serviceContracts/serviceContract[@restful='false']">
			<h4>SOAP Services</h4>
			<table cellspacing="0" cellpadding="5" width="100%">
				<tr><th>Name</th><th>Description</th></tr>
				<xsl:apply-templates select="services/service" mode="tocSOAP">
					<xsl:sort select="@type" />
				</xsl:apply-templates>
			</table>
		</xsl:if>

		<xsl:if test="serviceContracts/serviceContract[@restful='true']">
			<h4>RESTful Services</h4>
			<table cellspacing="0" cellpadding="5" width="100%">
				<tr><th>Name</th><th>Description</th></tr>
				<xsl:apply-templates select="services/service" mode="tocREST">
					<xsl:sort select="@type" />
				</xsl:apply-templates>
			</table>
		</xsl:if>

		<xsl:if test="types/type">
			<h4>Types</h4>
			<table cellspacing="0" cellpadding="5" width="100%">
				<tr><th>Name</th><th>Namespace</th><th>Description</th></tr>
				<xsl:apply-templates select="types/type[@namespace != 'http://schemas.microsoft.com/2003/10/Serialization/']" mode="toc">
					<xsl:sort select="@namespace" />
					<xsl:sort select="@name" />
				</xsl:apply-templates>
			</table>
		</xsl:if>

		<br/><hr/>

		<xsl:if test="serviceContracts/serviceContract[@restful='false']">	
			<h3 class="SmallMargin">SOAP Services</h3>
			<hr/>
			<xsl:apply-templates select="services/service" mode="SOAP">
				<xsl:sort select="@type" />
			</xsl:apply-templates>
		</xsl:if>


		<xsl:if test="serviceContracts/serviceContract[@restful='true']">
			<h3 class="SmallMargin">RESTful Services</h3>
			<hr/>
			<xsl:apply-templates select="services/service" mode="REST">
				<xsl:sort select="@type" />
			</xsl:apply-templates>
		</xsl:if>

		<xsl:if test="types/type">
			<h3 class="SmallMargin">Types</h3>
			<hr/>
			<xsl:apply-templates select="types/type[@namespace != 'http://schemas.microsoft.com/2003/10/Serialization/']" mode="definition">
				<xsl:sort select="@namespace" />
				<xsl:sort select="@name" />
			</xsl:apply-templates>
		</xsl:if>

		<br/>
      
		</body>
		</html>

	</xsl:template>

	<xsl:template match="services/service" mode="tocSOAP">
		<xsl:variable name="type" select="contracts/contract/." />
		<xsl:apply-templates select="/doc/serviceContracts/serviceContract[@type=$type and @restful='false']" mode="toc" />
	</xsl:template>

	<xsl:template match="services/service" mode="tocREST">
		<xsl:variable name="type" select="contracts/contract/." />
		<xsl:apply-templates select="/doc/serviceContracts/serviceContract[@type=$type and @restful='true']" mode="toc" />
	</xsl:template>

	<xsl:template match="/doc/serviceContracts/serviceContract" mode="toc" >
		<tr>
			<td><a href="#{@id}"><xsl:value-of select="@name" /></a></td>
			<td width="100%">
				<xsl:apply-templates select="comments" mode="summary" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="/doc/types/type" mode="toc" >
		<tr>
			<td><a href="#{@id}"><xsl:value-of select="@name" /></a></td>
			<td><xsl:value-of select="@namespace" /></td>
			<td width="100%">
				<xsl:apply-templates select="comments" mode="summary" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="services/service" mode="SOAP">
		<xsl:variable name="type" select="contracts/contract/." />
		<xsl:apply-templates select="/doc/serviceContracts/serviceContract[@type=$type and @restful='false']" mode="definitionSOAP" />
	</xsl:template>

	<xsl:template match="services/service" mode="REST">
		<xsl:variable name="type" select="contracts/contract/." />
		<xsl:apply-templates select="/doc/serviceContracts/serviceContract[@type=$type and @restful='true']" mode="definitionREST" />
	</xsl:template>

	<xsl:template match="/doc/serviceContracts/serviceContract" mode="definitionSOAP" >
		<div>
			<a id="{@id}"><h4><xsl:value-of select="@name" /></h4></a>
			
			<xsl:variable name="contactType" select="@type" />	
			<h5><xsl:value-of select="/doc/services/service[contracts/contract/. = $contactType]/website/path/." />	</h5>
			
			<xsl:apply-templates select="comments" mode="summary" />
			
			<h5>Operations</h5>
			<table cellspacing="0" cellpadding="5" width="100%">
				<tr><th>Name</th><th>Description</th></tr>
				<xsl:apply-templates select="operations/operation" mode="operationSOAP">
					<xsl:sort select="@methodName"/>
				</xsl:apply-templates>
			</table>
			
			<xsl:apply-templates select="comments" mode="details" />
		</div>
		<br/><br/><hr/>
		<xsl:apply-templates select="operations/operation" mode="operationDefinitionSOAP">
			<xsl:sort select="@methodName"/>
		</xsl:apply-templates>
	</xsl:template>

	<xsl:template match="/doc/serviceContracts/serviceContract" mode="definitionREST" >
		<div>
			<h4><a id="{@id}"><xsl:value-of select="@name" /></a></h4>
			<xsl:apply-templates select="comments" mode="summary" />
			
			<h5>Resources</h5>
			<table cellspacing="0" cellpadding="5" width="100%">
				<tr><th>Name</th><th>Uri</th><th>Method</th><th>Description</th></tr>
				<xsl:apply-templates select="operations/operation" mode="operationREST">
					<xsl:sort select="@restUriTemplate"/>
				</xsl:apply-templates>
			</table>		
			
			<xsl:apply-templates select="comments" mode="details" />
		</div>
		<br/><br/><hr/>
		<xsl:apply-templates select="operations/operation" mode="operationDefinitionREST">
			<xsl:sort select="@restUriTemplate"/>
		</xsl:apply-templates>
	</xsl:template>

	<xsl:template match="/doc/types/type" mode="definition" >
		<div>
			<a id="{@id}"><h4>
				<xsl:value-of select="@name" />
				<xsl:choose>
					<xsl:when test="@class='Enumeration'">
						<xsl:text> Enumeration</xsl:text>
					</xsl:when>
					<xsl:when test="@class='Collection'">
						<xsl:text> Array</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text> Type</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</h4></a>
			<xsl:apply-templates select="comments" mode="summary" />
			
			<xsl:if test="members/member">
				<h5>Members</h5>		
				<table cellspacing="0" cellpadding="5" width="100%">
					<tr><th nowrap="nowrap">Member Name</th><th>Type</th><th>Required</th><th>Description</th></tr>
					<xsl:apply-templates select="members/member" />
				</table>
			</xsl:if>
			
			<xsl:if test="options/option">
				<h5>Options</h5>
				<table cellspacing="0" cellpadding="5" width="100%">
					<tr><th>Option</th><th>Description</th></tr>
					<xsl:apply-templates select="options/option" />
				</table>
			</xsl:if>
			
			<xsl:if test="@class='Primitive'">
				<h5>Base Type</h5>
				<table cellspacing="0" cellpadding="5" width="100%">
					<tr><th nowrap="nowrap">Type</th><th>Namespace</th></tr>
					<tr>
						<td>
							<xsl:choose>
								<xsl:when test="@relatedTypeNamespace != 'http://www.w3.org/2001/XMLSchema' and @relatedTypeNamespace != 'http://schemas.microsoft.com/2003/10/Serialization/'">
									<a href="#{@relatedTypeId}"><xsl:value-of select="@relatedType" /></a>
								</xsl:when>
								<xsl:otherwise><xsl:value-of select="@relatedType" /></xsl:otherwise>
							</xsl:choose>					
						</td>
						<td width="100%"><xsl:value-of select="@relatedTypeNamespace" /></td>
					</tr>
				</table>
			</xsl:if>
			
			<xsl:if test="@class='Collection'">
				<h5>Array Element</h5>		
				<table cellspacing="0" cellpadding="5" width="100%">
					<tr><th nowrap="nowrap">Element Name</th><th>Type</th></tr>
					<tr>
						<td><xsl:value-of select="@relatedName" /></td>
						<td width="100%">
							<xsl:choose>
								<xsl:when test="@relatedTypeNamespace != 'http://www.w3.org/2001/XMLSchema' and @relatedTypeNamespace != 'http://schemas.microsoft.com/2003/10/Serialization/'">
									<a href="#{@relatedTypeId}"><xsl:value-of select="@relatedType" /></a>
								</xsl:when>
								<xsl:otherwise><xsl:value-of select="@relatedType" /></xsl:otherwise>
							</xsl:choose>					
						</td>
					</tr>
				</table>
			</xsl:if>
			
			<xsl:if test="@class='Complex' or @class='Collection' or @class='Enumeration'">
				<h5>RESTful Xml Representation</h5>	
<PRE style="background-color:#DDDDDD;padding:5px">
&lt;<xsl:value-of select="@name" />&gt;<xsl:choose>
<xsl:when test="@class='Enumeration'"><xsl:value-of select="options/option/@value" /></xsl:when>
<xsl:when test="@class='Collection'"><xsl:text disable-output-escaping="yes"><![CDATA[
]]></xsl:text>
<xsl:text disable-output-escaping="yes"><![CDATA[    ]]></xsl:text>&lt;<xsl:value-of select="@relatedName" />&gt;...&lt;/<xsl:value-of select="@relatedName" />&gt;
<xsl:text disable-output-escaping="yes"><![CDATA[    ]]></xsl:text>&lt;<xsl:value-of select="@relatedName" />&gt;...&lt;/<xsl:value-of select="@relatedName" />&gt;
<xsl:text disable-output-escaping="yes"><![CDATA[    ]]></xsl:text>&lt;<xsl:value-of select="@relatedName" />&gt;...&lt;/<xsl:value-of select="@relatedName" />&gt;
<xsl:text disable-output-escaping="yes"><![CDATA[    ]]></xsl:text>...
</xsl:when>
<xsl:otherwise><xsl:text disable-output-escaping="yes"><![CDATA[
]]></xsl:text>
<xsl:for-each select="members/member">
<xsl:text disable-output-escaping="yes"><![CDATA[    ]]></xsl:text>&lt;<xsl:value-of select="@name" />&gt;...&lt;/<xsl:value-of select="@name" />&gt;
</xsl:for-each>
</xsl:otherwise></xsl:choose>&lt;/<xsl:value-of select="@name" />&gt;	
</PRE>
			</xsl:if>
			
			<xsl:apply-templates select="comments" mode="details" />
		</div>
    <br/><hr/>
	</xsl:template>

	<xsl:template match="members/member" >
		<tr>
			<td nowrap="nowrap"><xsl:value-of select="@name" /></td>
			<td nowrap="nowrap">
				<xsl:choose>
					<xsl:when test="@typeNamespace != 'http://www.w3.org/2001/XMLSchema' and @typeNamespace != 'http://schemas.microsoft.com/2003/10/Serialization/'">
						<a href="#{@typeId}"><xsl:value-of select="@type" /></a>
					</xsl:when>
					<xsl:otherwise><xsl:value-of select="@type" /></xsl:otherwise>
				</xsl:choose>			
			</td>
		  <td>
			  <xsl:choose>
				  <xsl:when test="@required = 'true'">Yes</xsl:when>
				  <xsl:otherwise>No</xsl:otherwise>
			  </xsl:choose>
		  </td>
			<td width="100%"><xsl:value-of select="comments/." /></td>
		</tr>
	</xsl:template>

	<xsl:template match="options/option" >
		<tr>
			<td nowrap="nowrap"><xsl:value-of select="@value" /></td>
			<td width="100%"><xsl:value-of select="comments/." /></td>
		</tr>
	</xsl:template>

	<xsl:template match="operations/operation" mode="operationSOAP" >
		<tr>
			<td nowrap="nowrap"><a href="#{@id}"><xsl:value-of select="@methodName" /></a>(<xsl:apply-templates select="parameters/parameter" mode="signature" />)</td>
			<td width="100%"><xsl:apply-templates select="comments" mode="summary" /></td>
		</tr>
	</xsl:template>

	<xsl:template match="operations/operation" mode="operationREST" >
		<tr>
			<td><a href="#{@id}"><xsl:value-of select="@methodName"/></a></td>
			<td nowrap="nowrap">			
				<xsl:variable name="contactType" select="../../@type" />
				<xsl:variable name="uriTemplate" select="/doc/services/service[contracts/contract/. = $contactType]/website/path/." />
				<xsl:value-of select="substring($uriTemplate, 0, string-length($uriTemplate)-3)" />
				<xsl:value-of select="@restUriTemplate" />				
			</td>
			<td><xsl:value-of select="@restMethod"/></td>
			<td width="100%"><xsl:apply-templates select="comments" mode="summary" /></td>
		</tr>
	</xsl:template>

	<xsl:template match="operations/operation" mode="operationDefinitionSOAP" >
		<div>
			<h4>
				<a id="{@id}"><xsl:value-of select="../../@name" />\<xsl:value-of select="@name" /></a>(<xsl:apply-templates select="parameters/parameter" mode="signature" />)
			</h4>
			
			<xsl:variable name="contactType" select="../../@type" />
			<h5><xsl:value-of select="/doc/services/service[contracts/contract/. = $contactType]/website/path/." />	</h5>
			
			<xsl:apply-templates select="comments" mode="summary" />
			
			<xsl:if test="parameters/parameter">
				<h5>Parameters</h5>
				<table style="border-style: none">
					<xsl:apply-templates select="parameters/parameter" />
				</table>
			</xsl:if>
			
			<xsl:if test="return/@type">
				<h5>Return</h5>
				<xsl:apply-templates select="return" />
			</xsl:if>

			<xsl:apply-templates select="comments" mode="details" />
		</div>
    <br/>
    <hr/>
	</xsl:template>

	<xsl:template match="operations/operation" mode="operationDefinitionREST" >
		<div>
			<a id="{@id}"><h4><xsl:value-of select="../../@name" />\<xsl:value-of select="@name" /></h4></a>
			
			<xsl:variable name="contactType" select="../../@type" />
			<xsl:variable name="uriTemplate" select="/doc/services/service[contracts/contract/. = $contactType]/website/path/." />	
			<h5>
				<xsl:value-of select="@restMethod" />
				<xsl:text> : </xsl:text>
				<xsl:value-of select="substring($uriTemplate, 0, string-length($uriTemplate)-3)" />
				<xsl:value-of select="@restUriTemplate" />
			</h5>		
			
			<xsl:apply-templates select="comments" mode="summary" />
			
			<xsl:if test="parameters/parameter[@restfulType='EntityBody']">
				<h5>Entity Body</h5>
				<table style="border-style: none">
					<xsl:apply-templates select="parameters/parameter[@restfulType='EntityBody']" />
				</table>
			</xsl:if>
			
			<xsl:if test="parameters/parameter[@restfulType='PathSegment']">
				<h5>Path Segments</h5>
				<table style="border-style: none">
					<xsl:apply-templates select="parameters/parameter[@restfulType='PathSegment']" />
				</table>
			</xsl:if>
			
			<xsl:if test="parameters/parameter[@restfulType='QueryString']">
				<h5>QueryString</h5>
				<table style="border-style: none">
					<xsl:apply-templates select="parameters/parameter[@restfulType='QueryString']" />
				</table>
			</xsl:if>
			
			<xsl:if test="return/@type">
				<h5>Response</h5>
				<xsl:apply-templates select="return" />
			</xsl:if>
			
			<xsl:apply-templates select="comments" mode="details" />
		</div>
    <br/><hr/>
	</xsl:template>
	
	<xsl:template match="parameters/parameter" >
	
		<tr><td style="border-style: none"><i><xsl:value-of select="@name" /></i></td></tr>
		<tr>
			<td style="border-style: none"/>
			<td style="border-style: none">
			Type: <xsl:choose>
					<xsl:when test="@xmlTypeNamespace != 'http://www.w3.org/2001/XMLSchema' and @xmlTypeNamespace != 'http://schemas.microsoft.com/2003/10/Serialization/'">
						<a href="#{@typeId}"><xsl:value-of select="@xmlType" /></a>
					</xsl:when>
					<xsl:otherwise><xsl:value-of select="@xmlType" /></xsl:otherwise>
				</xsl:choose>
			</td>
		</tr>
		<tr><td style="border-style: none"/>
			<td style="border-style: none"><xsl:apply-templates select="comments" mode="summary" /></td>
		</tr>
	
	</xsl:template>
	
	<xsl:template match="return" >
		<table style="border-style: none">	
			<tr>
				<td style="border-style: none">
				Type: <xsl:choose>
						<xsl:when test="@xmlTypeNamespace != 'http://www.w3.org/2001/XMLSchema' and @xmlTypeNamespace != 'http://schemas.microsoft.com/2003/10/Serialization/'">
							<a href="#{@typeId}"><xsl:value-of select="@xmlType" /></a>
						</xsl:when>
						<xsl:otherwise><xsl:value-of select="@xmlType" /></xsl:otherwise>
					</xsl:choose>
				</td>
			</tr>
			<tr>
				<td style="border-style: none"><xsl:apply-templates select="comments" mode="summary" /></td>
			</tr>
		</table>
	</xsl:template>
	
	<xsl:template match="parameters/parameter" mode="signature">
	
		<xsl:choose>
			<xsl:when test="@xmlTypeNamespace != 'http://www.w3.org/2001/XMLSchema' and @xmlTypeNamespace != 'http://schemas.microsoft.com/2003/10/Serialization/'">
				<a href="#{@typeId}"><xsl:value-of select="@name" /></a>
			</xsl:when>
			<xsl:otherwise><xsl:value-of select="@name" /></xsl:otherwise>
		</xsl:choose>
		<xsl:if test="position() != last()"><xsl:text>, </xsl:text></xsl:if>
	
	</xsl:template>

	<!-- Xml Comments -->

	<xsl:template match="comments" mode="summary">
		<xsl:if test="summary">
			<p><xsl:apply-templates select="summary"/></p>
		</xsl:if>
	</xsl:template>

	<xsl:template match="comments" mode="details">
		<xsl:if test="remarks or value">
			<h5>Remarks</h5>
			<xsl:if test="remarks">
				<p><xsl:apply-templates select="remarks"/></p>
			</xsl:if>
			<xsl:if test="value">
				<p><xsl:apply-templates select="value"/></p>
			</xsl:if>
		</xsl:if>
		<xsl:if test="example">
			<h5>Example</h5>
			<xsl:if test="example">
				<p><xsl:apply-templates select="example"/></p>
			</xsl:if>
		</xsl:if>
		<xsl:if test="exception">
			<h5>Faults</h5>
			<ul>
				<xsl:for-each select="exception">
					<LI><xsl:value-of select="."/></LI>
				</xsl:for-each>
			</ul>
		</xsl:if>
		<xsl:if test="seealso">
			<h5>See Also</h5>
			<span style="padding-left: 20px">
				<xsl:for-each select="seealso">
					<a href="#{@cref}"><xsl:value-of select="."/></a>
					<xsl:if test="position() != last()"><xsl:text>, </xsl:text></xsl:if>
				</xsl:for-each>
			</span>
		</xsl:if>
	</xsl:template>

	<xsl:template match="code">
	  <PRE style="background-color:#DDDDDD;padding:5px"><xsl:apply-templates /></PRE>
	</xsl:template> 

	<xsl:template match="c">
	  <span style="font-family:Courier New;font-size:9pt;color:#006600"><xsl:value-of select="."/></span>
	</xsl:template>
	
	<xsl:template match="paramref">
	  <span style="font-family:Courier New;font-size:9pt;color:#006600"><xsl:value-of select="@name"/></span>
	</xsl:template>
  
	<xsl:template match="para">
	  <P><xsl:apply-templates /></P>
	</xsl:template>

	<xsl:template match="see">
	  <a href="#{@cref}"><xsl:value-of select="." />
      <xsl:text disable-output-escaping="yes"><![CDATA[]]></xsl:text>
    </a>
	</xsl:template>

	<xsl:template match="list">
	  <xsl:choose>
		<xsl:when test="@type='bullet'">
		  <UL>
		  <xsl:for-each select="listheader">
			<LI><strong><xsl:value-of select="term"/>: </strong><xsl:value-of select="description"/></LI>
		  </xsl:for-each>
		  <xsl:for-each select="item">
			<LI><strong><xsl:value-of select="term"/>: </strong><xsl:value-of select="description"/></LI>
		  </xsl:for-each>
		  </UL>
		</xsl:when>
		<xsl:when test="@type='number'">
		  <OL>
		  <xsl:for-each select="listheader">
			<LI><strong><xsl:value-of select="term"/>: </strong><xsl:value-of select="description"/></LI>
		  </xsl:for-each>
		  <xsl:for-each select="item">
			<LI><strong><xsl:value-of select="term"/>: </strong><xsl:value-of select="description"/></LI>
		  </xsl:for-each>
		  </OL>
		</xsl:when>
		<xsl:when test="@type='table'">
		  <TABLE>
		  <xsl:for-each select="listheader">
			<TR>
			  <TH><xsl:value-of select="term"/></TH>
			  <TH><xsl:value-of select="description"/></TH>
			</TR>
		  </xsl:for-each>
		  <xsl:for-each select="item">
			<TR>
			  <TD><strong><xsl:value-of select="term"/></strong></TD>
			  <TD><xsl:value-of select="description"/></TD>
			</TR>
		  </xsl:for-each>
		  </TABLE>
		</xsl:when>
	  </xsl:choose>
	</xsl:template>  
  
</xsl:stylesheet>
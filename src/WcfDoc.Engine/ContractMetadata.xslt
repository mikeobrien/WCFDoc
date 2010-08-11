<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/"
  xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:wsx="http://schemas.xmlsoap.org/ws/2004/09/mex" >

  <xsl:output method="xml" omit-xml-declaration="no" />

  <xsl:template match="/wsx:Metadata">
    <metadata>
      <types>
        <xsl:apply-templates select="wsx:MetadataSection/xs:schema/xs:simpleType" />
        <xsl:apply-templates select="wsx:MetadataSection/xs:schema/xs:complexType" />
      </types>
      <services>
        <xsl:apply-templates select="wsx:MetadataSection/wsdl:definitions/wsdl:portType" />
      </services>
    </metadata>
  </xsl:template>

  <xsl:template match="wsx:MetadataSection/wsdl:definitions/wsdl:portType">
    <service>
      <xsl:attribute name="name">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <xsl:attribute name="namespace">
        <xsl:value-of select="../@targetNamespace"/>
      </xsl:attribute>
      <xsl:apply-templates select="wsdl:operation" />
    </service>
  </xsl:template>

  <xsl:template match="wsdl:operation">
    <operation>
      <xsl:attribute name="name">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <parameters>
        <xsl:apply-templates select="wsdl:input" />
      </parameters>
      <return>
        <xsl:apply-templates select="wsdl:output" />
      </return>
    </operation>
  </xsl:template>

  <xsl:template match="wsdl:input">
    <xsl:variable name="name">
      <xsl:choose>
        <xsl:when test="contains(@message, ':')">
          <xsl:value-of select="substring-after(@message, ':')" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="@message" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="namespace" select="../../../@targetNamespace" />
    <xsl:apply-templates select="/wsx:Metadata/wsx:MetadataSection/wsdl:definitions[@targetNamespace=$namespace]/wsdl:message[@name=$name]/wsdl:part[@name='parameters']" mode="parameter" />
  </xsl:template>

  <xsl:template match="wsdl:output">
    <xsl:variable name="name">
      <xsl:choose>
        <xsl:when test="contains(@message, ':')">
          <xsl:value-of select="substring-after(@message, ':')" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="@message" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="namespace" select="../../../@targetNamespace" />
    <xsl:apply-templates select="/wsx:Metadata/wsx:MetadataSection/wsdl:definitions[@targetNamespace=$namespace]/wsdl:message[@name=$name]/wsdl:part[@name='parameters']" mode="return" />
  </xsl:template>

  <xsl:template match="/wsx:Metadata/wsx:MetadataSection/wsdl:definitions/wsdl:message/wsdl:part" mode="parameter" >
    <xsl:variable name="name">
      <xsl:choose>
        <xsl:when test="contains(@element, ':')">
          <xsl:value-of select="substring-after(@element, ':')" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="@element" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="namespace" select="../../@targetNamespace" />
    <xsl:apply-templates select="/wsx:Metadata/wsx:MetadataSection/xs:schema[@targetNamespace=$namespace or ($namespace = '' and not(@targetNamespace))]/xs:element[@name=$name]/xs:complexType/xs:sequence/xs:element" mode="parameter" />
  </xsl:template>

  <xsl:template match="/wsx:Metadata/wsx:MetadataSection/wsdl:definitions/wsdl:message/wsdl:part" mode="return" >
    <xsl:variable name="name">
      <xsl:choose>
        <xsl:when test="contains(@element, ':')">
          <xsl:value-of select="substring-after(@element, ':')" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="@element" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="namespace" select="../../@targetNamespace" />
    <xsl:apply-templates select="/wsx:Metadata/wsx:MetadataSection/xs:schema[@targetNamespace=$namespace or ($namespace = '' and not(@targetNamespace))]/xs:element[@name=$name]/xs:complexType/xs:sequence/xs:element" mode="return" />
  </xsl:template>

  <xsl:template match="/wsx:Metadata/wsx:MetadataSection/xs:schema/xs:element/xs:complexType/xs:sequence/xs:element" mode="parameter" >
    <parameter name="{@name}">
      <xsl:variable name="type">
        <xsl:choose>
          <xsl:when test="contains(@type, ':')">
            <xsl:value-of select="substring-after(@type, ':')" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="@type" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>
      <xsl:variable name="namespacePrefix" select="substring-before(@type, ':')"/>
      <xsl:variable name="namespace" select="namespace::*[local-name(.)=$namespacePrefix]"/>
      <xsl:attribute name="type">
        <xsl:value-of select="$type" />
      </xsl:attribute>
      <xsl:attribute name="typeNamespace">
        <xsl:value-of select="$namespace" />
      </xsl:attribute>
      <xsl:attribute name="nillable">
        <xsl:choose>
          <xsl:when test="@nillable">
            <xsl:value-of select="@nillable" />
          </xsl:when>
          <xsl:otherwise>false</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
    </parameter>
  </xsl:template>

  <xsl:template match="/wsx:Metadata/wsx:MetadataSection/xs:schema/xs:element/xs:complexType/xs:sequence/xs:element" mode="return" >
    <xsl:variable name="type">
      <xsl:choose>
        <xsl:when test="contains(@type, ':')">
          <xsl:value-of select="substring-after(@type, ':')" />
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="@type" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="namespacePrefix" select="substring-before(@type, ':')"/>
    <xsl:variable name="namespace" select="namespace::*[local-name(.)=$namespacePrefix]"/>
    <xsl:attribute name="type">
      <xsl:value-of select="$type" />
    </xsl:attribute>
    <xsl:attribute name="typeNamespace">
      <xsl:value-of select="$namespace" />
    </xsl:attribute>
    <xsl:attribute name="nillable">
      <xsl:choose>
        <xsl:when test="@nillable">
          <xsl:value-of select="@nillable" />
        </xsl:when>
        <xsl:otherwise>false</xsl:otherwise>
      </xsl:choose>
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="wsx:MetadataSection/xs:schema/xs:simpleType">
    <type name="{@name}" namespace="{../@targetNamespace}">
      <xsl:if test="xs:restriction/xs:enumeration">
        <xsl:attribute name="class">Enumeration</xsl:attribute>
        <xsl:attribute name="relatedType">
          <xsl:choose>
            <xsl:when test="contains(xs:restriction/@base, ':')">
              <xsl:value-of select="substring-after(xs:restriction/@base, ':')" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="xs:restriction/@base" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:attribute>
        <xsl:variable name="namespacePrefix" select="substring-before(xs:restriction/@base, ':')"/>
        <xsl:variable name="namespace" select="namespace::*[local-name(.)=$namespacePrefix]"/>
        <xsl:attribute name="relatedTypeNamespace">
          <xsl:value-of select="$namespace" />
        </xsl:attribute>
      </xsl:if>
      <xsl:if test="not(xs:restriction/xs:enumeration)">
        <xsl:attribute name="class">Primitive</xsl:attribute>
        <xsl:attribute name="relatedType">
          <xsl:choose>
            <xsl:when test="contains(xs:restriction/@base, ':')">
              <xsl:value-of select="substring-after(xs:restriction/@base, ':')" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="xs:restriction/@base" />
            </xsl:otherwise>
          </xsl:choose>            
        </xsl:attribute>
        <xsl:variable name="namespacePrefix" select="substring-before(xs:restriction/@base, ':')"/>
        <xsl:variable name="namespace" select="namespace::*[local-name(.)=$namespacePrefix]"/>
        <xsl:attribute name="relatedTypeNamespace">
          <xsl:value-of select="$namespace" />
        </xsl:attribute>
      </xsl:if>
      <options>
        <xsl:if test="xs:restriction/xs:enumeration">
            <xsl:apply-templates select="xs:restriction/xs:enumeration" />
        </xsl:if>
      </options>
      <members/>
    </type>
  </xsl:template>

  <xsl:template match="xs:restriction/xs:enumeration">
    <option value="{@value}" />
  </xsl:template>

  <xsl:template match="wsx:MetadataSection/xs:schema/xs:complexType">
      <type name="{@name}" namespace="{../@targetNamespace}">
        <xsl:if test="xs:sequence/xs:element/@maxOccurs='unbounded'">
          <xsl:apply-templates select="xs:sequence/xs:element" mode="collection"/>
        </xsl:if>
        <xsl:if test="not(xs:sequence/xs:element/@maxOccurs='unbounded')">
          <xsl:attribute name="class">Complex</xsl:attribute>
        </xsl:if>
        <members>
          <xsl:if test="not(xs:sequence/xs:element/@maxOccurs='unbounded')">
              <xsl:apply-templates select="xs:sequence/xs:element" mode="member"/>
          </xsl:if>
        </members>
        <options/>
    </type>
  </xsl:template>

  <xsl:template match="xs:sequence/xs:element" mode="collection">
    <xsl:attribute name="class">Collection</xsl:attribute>
    <xsl:attribute name="relatedName">
      <xsl:value-of select="@name"/>
    </xsl:attribute>
    <xsl:attribute name="relatedType">
      <xsl:choose>
        <xsl:when test="contains(@type, ':')">
          <xsl:value-of select="substring-after(@type, ':')" /></xsl:when>
        <xsl:otherwise><xsl:value-of select="@type" /></xsl:otherwise>
      </xsl:choose>
    </xsl:attribute>
    <xsl:variable name="namespacePrefix" select="substring-before(@type, ':')"/>
    <xsl:variable name="namespace" select="namespace::*[local-name(.)=$namespacePrefix]"/>
    <xsl:attribute name="relatedTypeNamespace">
      <xsl:value-of select="$namespace" />
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="xs:sequence/xs:element" mode="member">
    <member name="{@name}">
      <xsl:attribute name="type">
        <xsl:choose>
          <xsl:when test="contains(@type, ':')">
            <xsl:value-of select="substring-after(@type, ':')" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="@type" />
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:attribute name="required">
        <xsl:choose>
          <xsl:when test="@minOccurs = '0'"><xsl:value-of select="'False'" /></xsl:when>
          <xsl:otherwise><xsl:value-of select="'True'" /></xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:variable name="namespacePrefix" select="substring-before(@type, ':')"/>
      <xsl:variable name="namespace" select="namespace::*[local-name(.)=$namespacePrefix]"/>
      <xsl:attribute name="typeNamespace">
        <xsl:value-of select="$namespace" />
      </xsl:attribute>
    </member>
  </xsl:template>

</xsl:stylesheet>

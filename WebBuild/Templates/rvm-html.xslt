<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="msxsl parse"
    xmlns:parse="http://www.rosivm.org/2014/parse/">

  <xsl:output indent="yes" encoding="utf-8" method="xml" omit-xml-declaration="yes"/>

  <xsl:param name="identchar" select="'&#160;&#160;&#160;&#160;'" />

  <xsl:template match="parse:Global">
    <code class="rvm">
      <ol>
        <xsl:apply-templates/>
      </ol>
    </code>
  </xsl:template>
  <xsl:template match="parse:Module">
    <xsl:param name="ident" select="''" />
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:apply-templates select="parse:module"/>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:Name"/>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:l-brac"/>
    </li>
    <xsl:apply-templates select="parse:Function | parse:Interface | parse:Class">
      <xsl:with-param name="ident" select="concat($ident, $identchar)" />
    </xsl:apply-templates>
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:apply-templates select="parse:r-brac"/>
    </li>
  </xsl:template>
  <xsl:template match="parse:Function">
    <xsl:param name="ident" select="''" />
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:if test ="parse:Visibility">
        <xsl:apply-templates select="parse:Visibility"/>
        <xsl:text>&#160;</xsl:text>
      </xsl:if>
      <xsl:apply-templates select="parse:function"/>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:Name"/>
      <xsl:apply-templates select="parse:l-paren"/>
      <xsl:apply-templates select="parse:Argument | parse:comma"/>
      <xsl:apply-templates select="parse:r-paren"/>
      <xsl:if test="parse:ReturnType">
        <xsl:text>&#160;</xsl:text>
        <xsl:apply-templates select="parse:ReturnType"/>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="parse:StatementBlock">
          <xsl:text>&#160;</xsl:text>
          <xsl:apply-templates select="parse:StatementBlock/parse:l-brac"/>
        </xsl:when>
        <xsl:when test="parse:semicolon">
          <xsl:apply-templates select="parse:semicolon" />
        </xsl:when>
      </xsl:choose>
    </li>
    <xsl:if test="parse:StatementBlock">
      <xsl:apply-templates select="parse:StatementBlock/parse:Statement">
        <xsl:with-param name="ident" select="concat($ident, $identchar)"/>
      </xsl:apply-templates>
      <li>
        <xsl:value-of select="$ident"/>
        <xsl:apply-templates select="parse:StatementBlock/parse:r-brac"/>
      </li>
    </xsl:if>
  </xsl:template>
  <xsl:template match="parse:Class">
    <xsl:param name="ident" select="''" />
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:if test="parse:Visibility">
        <xsl:apply-templates select="parse:Visibility"/>
        <xsl:text>&#160;</xsl:text>
      </xsl:if>
      <xsl:apply-templates select="parse:class"/>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:Name"/>
      <xsl:if test="parse:Extends">
        <xsl:text>&#160;</xsl:text>
        <xsl:apply-templates select="parse:Extends" />
      </xsl:if>
      <xsl:if test="parse:Implements">
        <xsl:text>&#160;</xsl:text>
        <xsl:apply-templates select="parse:Implements" />
      </xsl:if>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:l-brac"/>
    </li>
    <xsl:apply-templates select="parse:Field | parse:Constructor | parse:Method">
      <xsl:with-param name="ident" select="concat($ident, $identchar)" />
    </xsl:apply-templates>
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:apply-templates select="parse:r-brac"/>
    </li>
  </xsl:template>
  <xsl:template match="parse:Extends">
    <xsl:apply-templates select="parse:extends" />
    <xsl:text>&#160;</xsl:text>
    <xsl:apply-templates select="parse:Type" />
  </xsl:template>
  <xsl:template match="parse:Implements">
    <xsl:apply-templates select="parse:implements" />
    <xsl:text>&#160;</xsl:text>
    <xsl:apply-templates select="parse:Types" />
  </xsl:template>
  <xsl:template match="parse:Types">
    <xsl:apply-templates select="*" mode="comma-list" />
  </xsl:template>
  <xsl:template match="parse:Interface">
    <xsl:param name="ident" select="''" />
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:if test="parse:Visibility">
        <xsl:apply-templates select="parse:Visibility"/>
        <xsl:text>&#160;</xsl:text>
      </xsl:if>
      <xsl:apply-templates select="parse:interface"/>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:Name"/>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:l-brac"/>
    </li>
    <xsl:apply-templates select="parse:Field | parse:Method">
      <xsl:with-param name="ident" select="concat($ident, $identchar)" />
    </xsl:apply-templates>
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:apply-templates select="parse:r-brac"/>
    </li>
  </xsl:template>
  <xsl:template match="parse:Field">
    <xsl:param name="ident" select="''" />
    <li>
    <xsl:value-of select="$ident"/>
    <xsl:apply-templates/>
    </li>
  </xsl:template>
  <xsl:template match="parse:Method">
    <xsl:param name="ident" select="''" />
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:if test ="parse:Visibility">
        <xsl:apply-templates select="parse:Visibility"/>
        <xsl:text>&#160;</xsl:text>
      </xsl:if>
      <xsl:apply-templates select="parse:method"/>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:Name"/>
      <xsl:apply-templates select="parse:l-paren"/>
      <xsl:apply-templates select="parse:Argument | parse:comma"/>
      <xsl:apply-templates select="parse:r-paren"/>
      <xsl:if test="parse:ReturnType">
        <xsl:text>&#160;</xsl:text>
        <xsl:apply-templates select="parse:ReturnType"/>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="parse:StatementBlock">
          <xsl:text>&#160;</xsl:text>
          <xsl:apply-templates select="parse:StatementBlock/parse:l-brac"/>
        </xsl:when>
        <xsl:when test="parse:semicolon">
          <xsl:apply-templates select="parse:semicolon" />
        </xsl:when>
      </xsl:choose>
    </li>
    <xsl:if test="parse:StatementBlock">
      <xsl:apply-templates select="parse:StatementBlock/parse:Statement">
        <xsl:with-param name="ident" select="concat($ident, $identchar)"/>
      </xsl:apply-templates>
      <li>
        <xsl:value-of select="$ident"/>
        <xsl:apply-templates select="parse:StatementBlock/parse:r-brac"/>
      </li>
    </xsl:if>
  </xsl:template>
  <xsl:template match="parse:ReturnType">
    <xsl:apply-templates select="parse:turns-in"/>
    <xsl:text>&#160;</xsl:text>
    <xsl:apply-templates select="parse:Type"/>
  </xsl:template>
  <xsl:template match="parse:Field">
    <xsl:param name="ident" select="''" />
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:apply-templates select="parse:var"/>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:Name"/>
      <xsl:apply-templates select="parse:colon"/>
      <xsl:text>&#160;</xsl:text>
      <xsl:apply-templates select="parse:Type"/>
      <xsl:apply-templates select="parse:semicolon"/>
    </li>
  </xsl:template>
  <xsl:template match="parse:StatementBlock">
    <xsl:param name="ident" select="''" />
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:apply-templates select="parse:l-brac"/>
    </li>
    <xsl:apply-templates select="parse:Statement">
      <xsl:with-param name="ident" select="concat($ident, $identchar)" />
    </xsl:apply-templates>
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:apply-templates select="parse:r-brac"/>
    </li>
  </xsl:template>
  <xsl:template match="parse:Statement">
    <xsl:param name="ident" select="''" />
    <li>
      <xsl:value-of select="$ident"/>
      <xsl:apply-templates>
        <xsl:with-param name="ident" select="$ident" />
      </xsl:apply-templates>
    </li>
  </xsl:template>
  <xsl:template match="parse:AddAssign">
    <!-- These must be "left" and "right". -->
    <xsl:apply-templates select="*[1]"/>
    <xsl:text>&#160;</xsl:text>
    <xsl:apply-templates select="parse:add-assign"/>
    <xsl:text>&#160;</xsl:text>
    <xsl:apply-templates select="*[3]"/>
  </xsl:template>
  
  <xsl:template match="parse:comma" mode="comma-list">
    <xsl:apply-templates select="." />
    <xsl:text>&#160;</xsl:text>
  </xsl:template>
  <xsl:template match="@* | node()" mode="comma-list">
    <xsl:apply-templates select="." />
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:param name="ident" select="''" />
    <xsl:choose>
      <xsl:when test="@prod">
        <!--<xsl:copy>-->
        <xsl:apply-templates>
          <xsl:with-param name="ident" select="$ident" />
        </xsl:apply-templates>
        <!--</xsl:copy>-->
      </xsl:when>
      <xsl:otherwise>
        <span class="{local-name()}">
          <xsl:value-of select="." />
        </span>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>

<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="msxsl"
    xmlns:parse="http://www.rosivm.org/2014/parse/">

  <xsl:output method="xml" indent="yes"/>

  <xsl:param name="identchars" select="'&#160;&#160;'" />

  <xsl:strip-space elements="*"/>

  <xsl:template match="/">
    <html>
      <style>
        .Visibility, .class, .module, .interface, .function, .method {color:#04A; font-weight:bold;}
        .Name {color:#A0A; font-weight:bold;}
        .Type {color:#00A; font-weight:bold;}
        .var, .turns-in {color:#AAA; font-weight:bold;}
        {color:#AAA; font-weight:bold;}
        .ConstantValue {color:#A00; font-weight:bold;}
        .if, .else {color:#600; font-weight:bold;}
      </style>
      <body>
        <code>
          <xsl:text>&#xa;</xsl:text>
          <xsl:apply-templates select="*">
            <xsl:with-param name="ident" select="''" />
          </xsl:apply-templates>
          <xsl:text>&#xa;</xsl:text>
        </code>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="node()">
    <xsl:param name="ident" select="''" />
    <xsl:choose>
      <xsl:when test="text()">
        <xsl:apply-templates select="." mode="terminal">
          <xsl:with-param name="ident" select="$ident" />
        </xsl:apply-templates>
      </xsl:when>
      <xsl:when test="node()">
        <xsl:apply-templates select="." mode="production">
          <xsl:with-param name="ident" select="$ident" />
        </xsl:apply-templates>
      </xsl:when>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="parse:Module" mode="production">
    <xsl:param name="ident" select="''" />

    <xsl:value-of select="$ident"/>

    <xsl:apply-templates select="parse:module" />
    <xsl:text> </xsl:text>
    <xsl:apply-templates select="parse:Name" />
    <xsl:text> </xsl:text>
    <xsl:apply-templates select="parse:l-brac" />
    <br/>
    <xsl:text>&#xa;</xsl:text>

    <xsl:apply-templates select="parse:Function | parse:Class | parse:Interface">
      <xsl:with-param name="ident" select="concat($identchars, $ident)" />
    </xsl:apply-templates>

    <xsl:value-of select="$ident"/>
    <xsl:apply-templates select="parse:r-brac" />
    <br/>
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>
  <xsl:template match="parse:Function">
    <xsl:param name="ident" select="''" />

    <xsl:value-of select="$ident"/>

    <xsl:if test="parse:Visibility">
      <xsl:apply-templates select="parse:Visibility" />
      <xsl:text> </xsl:text>
    </xsl:if>
    <xsl:apply-templates select="parse:function" />
    <xsl:text> </xsl:text>
    <xsl:apply-templates select="parse:Name" />

    <xsl:apply-templates select="parse:l-paren" />
    <xsl:apply-templates select="parse:Parameter | parse:Parameters" />
    <xsl:apply-templates select="parse:r-paren" />
    <xsl:apply-templates select="parse:ReturnType" />

    <xsl:text> </xsl:text>
    <xsl:apply-templates select="parse:StatementBlock">
      <xsl:with-param name="ident" select="$ident" />
    </xsl:apply-templates>

    <br/>
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>
  <xsl:template match="parse:Method">
    <xsl:param name="ident" select="''" />

    <xsl:value-of select="$ident"/>

    <xsl:if test="parse:Visibility">
      <xsl:apply-templates select="parse:Visibility" />
      <xsl:text> </xsl:text>
    </xsl:if>
    <xsl:apply-templates select="parse:method" />
    <xsl:text> </xsl:text>
    <xsl:apply-templates select="parse:Name" />

    <xsl:apply-templates select="parse:l-paren" />
    <xsl:apply-templates select="parse:Parameter | parse:Parameters" />
    <xsl:apply-templates select="parse:r-paren" />
    <xsl:apply-templates select="parse:ReturnType" />

    <xsl:if test="parse:StatementBlock">
      <xsl:text> </xsl:text>
      <xsl:apply-templates select="parse:StatementBlock">
        <xsl:with-param name="ident" select="$ident" />
      </xsl:apply-templates>
    </xsl:if>
    <xsl:apply-templates select="parse:semicolon">
      <xsl:with-param name="ident" select="$ident" />
    </xsl:apply-templates>
    <br/>
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>
  <xsl:template match="parse:Class" mode="production">
    <xsl:param name="ident" select="''" />

    <xsl:value-of select="$ident"/>

    <xsl:if test="parse:Visibility">
      <xsl:apply-templates select="parse:Visibility" />
      <xsl:text> </xsl:text>
    </xsl:if>
    <xsl:apply-templates select="parse:class" />
    <xsl:text> </xsl:text>
    <xsl:apply-templates select="parse:Name" />
    <xsl:text> </xsl:text>

    <xsl:apply-templates select="parse:l-brac" />
    <br/>
    <xsl:text>&#xa;</xsl:text>

    <xsl:apply-templates select="parse:Var | parse:Method">
      <xsl:with-param name="ident" select="concat($identchars, $ident)" />
    </xsl:apply-templates>

    <xsl:value-of select="$ident"/>
    <xsl:apply-templates select="parse:r-brac" />
    <br/>
    <xsl:text>&#xa;</xsl:text>
  </xsl:template>
  <xsl:template match="parse:Interface" mode="production">
    <xsl:param name="ident" select="''" />

    <xsl:value-of select="$ident"/>

    <xsl:if test="parse:Visibility">
      <xsl:apply-templates select="parse:Visibility" />
      <xsl:text> </xsl:text>
    </xsl:if>
    <xsl:apply-templates select="parse:interface" />
    <xsl:text> </xsl:text>
    <xsl:apply-templates select="parse:Name" />
    <xsl:text> </xsl:text>

    <!-- extends -->

    <xsl:apply-templates select="parse:l-brac" />
    <br/>
    <xsl:text>&#xa;</xsl:text>

    <xsl:apply-templates select="parse:Var | parse:Method">
      <xsl:with-param name="ident" select="concat($identchars, $ident)" />
    </xsl:apply-templates>

    <xsl:value-of select="$ident"/>
    <xsl:apply-templates select="parse:r-brac" />
    <br/>
    <xsl:text>&#xa;</xsl:text>

  </xsl:template>
  
  <xsl:template match="parse:StatementBlock" mode="production">
    <xsl:param name="ident" select="''" />

    <xsl:apply-templates select="parse:l-brac">
      <xsl:with-param name="ident" select="$ident" />
    </xsl:apply-templates>
    <br/>
    <xsl:text>&#xa;</xsl:text>

    <xsl:for-each select="*">
      <xsl:choose>
        <xsl:when test="position() = 1">
        </xsl:when>
        <xsl:when test="position() = count(../*)">
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="concat($identchars, $ident)"/>
          <xsl:apply-templates select=".">
            <xsl:with-param name="ident" select="concat($identchars, $ident)" />
          </xsl:apply-templates>
          <br/>
          <xsl:text>&#xa;</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>

    <xsl:value-of select="$ident"/>
    <xsl:apply-templates select="parse:r-brac">
      <xsl:with-param name="ident" select="$ident" />
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="parse:comma | parse:colon" mode="terminal">
    <xsl:param name="ident" />
    <xsl:call-template name="print-terminal">
      <xsl:with-param name="ident" select="$ident" />
    </xsl:call-template>
    <xsl:text>&#160;</xsl:text>
  </xsl:template>
  <xsl:template match="parse:turns-in" mode="terminal">
    <xsl:param name="ident" />

    <xsl:text>&#160;</xsl:text>
    <xsl:call-template name="print-terminal">
      <xsl:with-param name="ident" select="$ident" />
    </xsl:call-template>
    <xsl:text>&#160;</xsl:text>
  </xsl:template>

  <xsl:template match="*" mode="terminal">
    <xsl:param name="ident" select="''" />
    <xsl:call-template name="print-terminal">
      <xsl:with-param name="ident" select="$ident" />
    </xsl:call-template>
  </xsl:template>
  <xsl:template match="*" mode="production">
    <xsl:param name="ident" select="''" />
    <xsl:call-template name="print-production">
      <xsl:with-param name="ident" select="$ident" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="print-terminal">
    <xsl:param name="ident" select="''" />
    <span class="{name()}">
      <xsl:value-of select="text()"/>
    </span>
  </xsl:template>
  <xsl:template name="print-production">
    <xsl:param name="ident" select="''" />
    <xsl:apply-templates select="*">
      <xsl:with-param name="ident" select="$ident" />
    </xsl:apply-templates>
  </xsl:template>

</xsl:stylesheet>

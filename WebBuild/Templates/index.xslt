<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:www="urn:web-build:xslt"
    exclude-result-prefixes="msxsl">

  <xsl:output indent="yes" encoding="utf-8" method="xml" omit-xml-declaration="yes"/>

  <xsl:template match="html">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="/style.css" />
        <link rel="icon" type="image/png" href="/favicon.png" />
        <xsl:apply-templates select="head/*" />
      </head>
      <body>
        <div id="navigation">
          <!-- This document may need to be defined in the processed doc. -->
          <xsl:apply-templates select="document('navigation.html')/html/body/*"/>
        </div>
        <div id="header">
          <xsl:apply-templates select="document('header.html')/html/body/*"/>
        </div>
        <div id="content">
          <xsl:apply-templates select="body/*" />
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="@class">
    <xsl:attribute name="class">
      <xsl:value-of select="."/>
      <xsl:if test="www:isinside(parent::a/@href)">
        <xsl:text> inside</xsl:text>
      </xsl:if>
      <xsl:if test="www:isselected(parent::a/@href)">
        <xsl:text> selected</xsl:text>
      </xsl:if>
    </xsl:attribute>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>

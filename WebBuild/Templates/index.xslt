<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="msxsl">

  <xsl:output indent="yes" encoding="utf-8" method="xml" omit-xml-declaration="yes"/>

  <xsl:template match="html">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="style.css" />
        <xsl:apply-templates select="head/*" />
      </head>
      <body>
        <div id="navigation">
          <xsl:apply-templates select="document('navigation.html')/html/body/*"/>
        </div>
        <div id="content">
          <xsl:apply-templates select="body/*" />
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>

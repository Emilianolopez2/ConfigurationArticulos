﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace MvcWebPage.MLAVID;

public partial class IT_INV_MENSUALES_LIN
{
    public int ID { get; set; }

    public string REFERENCIA { get; set; }

    public int? CODARTICULO { get; set; }

    public string UNIDADMEDIDA { get; set; }

    public string DESCRIPCION { get; set; }

    public double? MULTIPLOPEDIR { get; set; }

    public double? MAXIMO_PEDIR { get; set; }

    public string DESCRIPCION_DEPTO { get; set; }

    public double? PRECIO_A { get; set; }

    public double? EXISTENCIA_ACTUAL { get; set; }

    public virtual IT_INV_MENSUALES_CAB IDNavigation { get; set; }
}
﻿using System;

namespace EdiClient.Services
{
    internal static class SqlConfiguratorService
    {
        internal static string DatesBetween(DateTime Date)
            => $" BETWEEN {OracleDateFormat(Date)} AND {OracleDateFormat(Date)} ";
        internal static string DatesBetween(DateTime DateFrom, DateTime DateTo) 
            => $" BETWEEN {OracleDateFormat(DateFrom)} AND {OracleDateFormat(DateTo)} ";
        internal static string OracleDateFormat(DateTime Date) => $" TO_DATE('{Date.Day}/{Date.Month}/{Date.Year}','DD/MM/YYYY') ";
        internal static string Sql_DateRange(string tableName, string fieldName, string sign, DateTime date1) 
            => $"{tableName}.{fieldName} = {OracleDateFormat(date1)}\n";
        internal static string Sql_DateRange(string shortTableName, string fieldName, string sign, DateTime date1, DateTime date2) 
            => $"{shortTableName}.{fieldName} {DatesBetween(date1, date2)}\n";
        internal static string Sql_SelectOrdersLocaterInBase() 
            => "SELECT ORDER_NUMBER FROM HPCSERVICE.EDI_DOC WHERE ID_TRADER IS NOT NULL AND ORDER_NUMBER IS NOT NULL";
        internal static string Sql_SelectDetailsFailed(string numbers) 
            => $"SELECT BUYER_ITEM_CODE FROM HPCSERVICE.EDI_DOC_DETAILS WHERE FAILED = 1 AND ID_EDI_DOC IN (SELECT ID FROM HPCSERVICE.EDI_DOC ED WHERE ED.ORDER_NUMBER IN({numbers}) AND ED.HAS_FAILED_DETAILS = 1)";
        internal static string Sql_SelectOrdersFailed()
            => "SELECT ID FROM HPCSERVICE.edi_doc WHERE HAS_FAILED_DETAILS = 1";
        internal static string Sql_SelectAllOrderIds() 
            => "SELECT ORDER_NUMBER FROM HPCSERVICE.EDI_DOC";
        internal static string Sql_UpdateEdiDocSetIsInEdiAsORDRSP(string code) =>
            $@"UPDATE hpcservice.EDI_DOC SET IS_IN_EDI_AS_ORDRSP = {OracleDateFormat(DateTime.UtcNow)} WHERE ORDER_NUMBER 
            in (SELECT ORDER_NUMBER FROM hpcservice.edi_doc WHERE ID_TRADER
            in (SELECT ID FROM abt.DOC_JOURNAL DJ WHERE CODE = '{code}'))";
        internal static string Sql_UpdateEdiDocSetIsInEdiAsDESADV(string code) =>
            $@"UPDATE HPCSERVICE.EDI_DOC SET IS_IN_EDI_AS_DESADV = {OracleDateFormat(DateTime.UtcNow)} WHERE ORDER_NUMBER 
            in (SELECT ORDER_NUMBER FROM HPCSERVICE.edi_doc WHERE ID_TRADER
            in (SELECT ID FROM ABT.DOC_JOURNAL DJ WHERE CODE = '{code}'))";
        internal static string Sql_SelectOrdrspDetails(string traderDocId)
        {
            return $@"SELECT 
    DGD.ID_DOC
  , DGD.ID_GOOD
  , DGD.QUANTITY
  , DGD.PRICE
  , DGD.ID_ITEM
  , DGD.ITEM_QUANTITY
  , DGD.ITEM_POS
  , DGD.DISCOUNT_RATE
  , DGD.DISCOUNT_SUMM
  , DGD.CHARGE_RATE
  , DGD.CHARGE_SUMM
  , DGD.LOCK_STATUS
  , DGD.ERR
  , DGD.PRIME
  , RG.ID
  , RG.SERT_NUM
  , RG.REG_NUM
  , RG.EXPIRING_DATE
  , RG.CODE
  , RG.NAME
  , RG.TAX
  , RG.ID_BASE_ITEM
  , RG.ID_DEFAULT_ITEM
  , RG.ID_ACCOUNT_CURRENCY
  , RG.ID_MANUFACTURER
  , RG.ID_ORGAN
  , RG.ID_COUNTRY
  , RG.CUSTOMS_NO
  , RG.ID_SUBDIVISION
  , RG.HAS_REMAIN
  , RG.OLDID
  , RG.GOOD_SIZE
  , RG.BAR_CODE
  , EDD.ID_EDI_DOC
  , EDD.LINE_NUMBER LineNumber
  , EDD.BUYER_ITEM_CODE BuyerItemCode
  , EDD.ITEM_DESCRIPTION ItemDescription
  , EDD.ORDERED_UNIT_NET_PRICE OrderedUnitNetPrice
  , EDD.ORDERED_UNIT_GROSS_PRICE orderedUnitGrossPrice
  , EDD.TAX_RATE TaxRate
  , EDD.REASON_CODE ReasonCode
  , EDD.EAN
  , EDD.GROSS_WEIGHT GrossWeight
  , EDD.NET_WEIGHT NetWeight
  , EDD.ORDERED_GROSS_AMOUNT OrderedGrossAmount
  , EDD.ORDERED_NET_AMOUNT OrderedNetAmount
  , EDD.ORDERED_PALLETS OrderedPallets
  , EDD.ORDERED_TAX_AMOUNT OrderedTaxAmount
  , EDD.ORDERED_UNIT_PACKSIZE OrderedUnitPacksize
  , EDD.SUPPLIER_ITEM_CODE SupplierItemCode
  , EDD.UNIT_OF_MEASURE UnitOfMeasure
  , EDD.VOLUME Volume
  , EDD.ORDERED_BOXES OrderedBoxes 
  , EDD.ORDERED_QUANTITY OrderedQuantity
FROM 
    ABT.DOC_GOODS_DETAILS DGD 
  , ABT.REF_GOODS RG
  , HPCSERVICE.EDI_DOC_DETAILS EDD
WHERE DGD.ID_GOOD = RG.ID
AND EDD.ID_GOOD = RG.ID
AND edd.ID_EDI_DOC = (SELECT id FROM HPCSERVICE.EDI_DOC WHERE ID_TRADER = {traderDocId} and rownum = 1)
AND DGD.ID_DOC = {traderDocId}";
        }
        internal static string Sql_SelectOrdrspHeader(DateTime date1, DateTime date2)
        {        
            return $@"SELECT 
    ED.ID
  , ED.ID_CUSTOMER
  , ED.DOC_TYPE
  , ED.ID_TRADER
  , ED.SENDER_ILN SenderIln
  , ED.PROMOTION_REFERENCE PromotionReference
  , ED.EXPECTED_DELIVERY_DATE ExpectedDeliveryDate
  , ED.PAYMENT_DUE_DATE PaymentDueDate
  , ED.DELIVERY_POINT_ILN DeliveryPointIln
  , ED.BUYER_ILN Buyeriln
  , ED.CUSTOMER_ILN CustomerIln
  , ED.SELLER_ILN SellerIln
  , ED.ORDER_CURRENCY OrderCurrency
  , ED.ORDER_DATE OrderDate
  , ED.ORDER_NUMBER orderNumber
  , ED.ORDER_PAYMENT_DUE_DATE OrderPaymentDueDate
  , ED.REMARKS Remarks
  , ED.TOTAL_GROSS_AMOUNT TotalGrossAmount
  , ED.IS_IN_EDI_AS_ORDRSP
  , ED.IS_IN_EDI_AS_DESADV
  , DJ.CODE
  , DJ.COMMENTS
  , DJ.DOC_DATETIME
  , DJ.ID_DOC_TYPE
  , DJ.ID_INSTANCE_SENDER
  , DJ.ID_INSTANCE_RECIEPIENT
  , DJ.ID_INSTANCE_OWNER
  , DJ.ID_DOC_MASTER
  , DJ.ERROR_STATUS
  , DJ.ACT_STATUS
  , DJ.LOCK_STATUS
  , DJ.USER_NAME
  , DJ.CREATE_INVOICE
  , DJ.DELIVERY_DATE
  , DJ.DELETED
  , DJ.UPDATED_PRICE
  , DJ.PAY_DELAY
  , DG.ID_AGENT
  , DG.ID_CURRENCY
  , DG.CURRENCY_RATE
  , DG.ID_PRICE_TYPE
  , DG.DISCOUNT_RATE
  , DG.DISCOUNT_SUMM
  , DG.TOTAL_SUMM
  , DG.ID_SELLER
  , DG.ID_CUSTOMER
  , DG.ID_STORE_SENDER
  , DG.ID_STORE_RECIEPIENT
  , DG.ID_SUBDIVISION
  , DG.ID_DOC_RETURN
  , DG.CHARGE_RATE
  , DG.CHARGE_SUMM
  , DG.IS_RETURN
  , DG.LOCK_STATUS
  , DG.DOC_PRECISION
  , DG.TOTAL_PRIME
  ,(SELECT COUNT(*) FROM ABT.DOC_GOODS_DETAILS DGD WHERE DGD.ID_DOC = DJ.ID) TOTAL_LINES
FROM 
  ABT.DOC_GOODS DG
  , ABT.DOC_JOURNAL DJ
  , HPCSERVICE.EDI_DOC ED
WHERE 1 = 1
  AND DG.ID_DOC = DJ.ID
  AND ED.ID_TRADER = DJ.ID
  AND DJ.ID IN (SELECT ID_TRADER FROM HPCSERVICE.EDI_DOC WHERE ID_TRADER IS NOT NULL)
  AND ED.IS_IN_EDI_AS_DESADV IS NULL
  AND DJ.ACT_STATUS >= 3
  AND DJ.DOC_DATETIME {DatesBetween(date1, date2)}
            ";
        }        
        internal static string Sql_SelectDesadvHeader(DateTime date1, DateTime date2)
        {
            return $@"SELECT 
    ED.ID
  , ED.ID_CUSTOMER
  , ED.DOC_TYPE
  , ED.ID_TRADER
  , ED.SENDER_ILN SenderIln
  , ED.PROMOTION_REFERENCE PromotionReference
  , ED.EXPECTED_DELIVERY_DATE ExpectedDeliveryDate
  , ED.PAYMENT_DUE_DATE PaymentDueDate
  , ED.DELIVERY_POINT_ILN DeliveryPointIln
  , ED.BUYER_ILN Buyeriln
  , ED.CUSTOMER_ILN CustomerIln
  , ED.SELLER_ILN SellerIln
  , ED.ORDER_CURRENCY OrderCurrency
  , ED.ORDER_DATE OrderDate
  , ED.ORDER_NUMBER orderNumber
  , ED.ORDER_PAYMENT_DUE_DATE OrderPaymentDueDate
  , ED.REMARKS Remarks
  , ED.TOTAL_GROSS_AMOUNT TotalGrossAmount
  , ED.IS_IN_EDI_AS_ORDRSP
  , ED.IS_IN_EDI_AS_DESADV
  , DJ.CODE
  , DJ.COMMENTS
  , DJ.DOC_DATETIME
  , DJ.ID_DOC_TYPE
  , DJ.ID_INSTANCE_SENDER
  , DJ.ID_INSTANCE_RECIEPIENT
  , DJ.ID_INSTANCE_OWNER
  , DJ.ID_DOC_MASTER
  , DJ.ERROR_STATUS
  , DJ.ACT_STATUS
  , DJ.LOCK_STATUS
  , DJ.USER_NAME
  , DJ.CREATE_INVOICE
  , DJ.DELIVERY_DATE
  , DJ.DELETED
  , DJ.UPDATED_PRICE
  , DJ.PAY_DELAY
  , DG.ID_AGENT
  , DG.ID_CURRENCY
  , DG.CURRENCY_RATE
  , DG.ID_PRICE_TYPE
  , DG.DISCOUNT_RATE
  , DG.DISCOUNT_SUMM
  , DG.TOTAL_SUMM
  , DG.ID_SELLER
  , DG.ID_CUSTOMER
  , DG.ID_STORE_SENDER
  , DG.ID_STORE_RECIEPIENT
  , DG.ID_SUBDIVISION
  , DG.ID_DOC_RETURN
  , DG.CHARGE_RATE
  , DG.CHARGE_SUMM
  , DG.IS_RETURN
  , DG.LOCK_STATUS
  , DG.DOC_PRECISION
  , DG.TOTAL_PRIME
  ,(SELECT COUNT(*) FROM ABT.DOC_GOODS_DETAILS DGD WHERE DGD.ID_DOC = DJ.ID) TOTAL_LINES
FROM 
  ABT.DOC_GOODS DG
  , ABT.DOC_JOURNAL DJ
  , HPCSERVICE.EDI_DOC ED
WHERE 1 = 1
  AND DG.ID_DOC = DJ.ID
  AND ED.ID_TRADER = DJ.ID
  AND DJ.ID IN (SELECT ID_TRADER FROM HPCSERVICE.EDI_DOC WHERE ID_TRADER IS NOT NULL)
  AND ED.IS_IN_EDI_AS_ORDRSP IS NOT NULL
  AND DJ.ACT_STATUS >= 4
  AND DJ.DOC_DATETIME {DatesBetween(date1, date2)}";
        }

        internal static string ToOracleDate(DateTime Date)
        {
            var day = Date.Day < 10 ? $"0{Date.Day}" : Date.Day.ToString();
            var mouth = Date.Month < 10 ? $"0{Date.Month}" : Date.Month.ToString();
            return $"{day}.{mouth}. {Date.Year} {Date.Hour}:{Date.Minute}:{Date.Second}";
        }

        internal static string Sql_SelectGoods()
            => $@"  SELECT rg.id ""Id""
                    , rg.code ""Code""
                    , rg.name ""Name""
                    , rbc.bar_code ""BarCode""
                    , rg.GOOD_SIZE ""GoodSize""
                    , (SELECT name FROM ABT.REF_CONTRACTORS WHERE ID = rg.id_manufacturer) ""Manufacturer""
                        , rg.ID_MANUFACTURER ""ManufacturerId""
                    , rg.SERT_NUM ""SertNum""
                    FROM abt.ref_goods rg, ABT.REF_BAR_CODES RBC
  WHERE RBC.id_good = rg.id";


        internal static string Sql_SelectFailedGoods()
            => $@"  SELECT id_good ""GoodId""
                    , EDD.ID_EDI_DOC ""EdiDocId""
                    , EDD.LINE_NUMBER ""LineNumber""
                    ,EDD.ITEM_DESCRIPTION ""ItemDescription""
                    ,EDD.EAN ""Ean""
                    ,EDD.BUYER_ITEM_CODE ""BuyerItemCode""
                    ,(SELECT ORDER_NUMBER||' от '||ORDER_DATE FROM HPCSERVICE.EDI_DOC WHERE id = edd.ID_EDI_DOC) ""OrderName""
                    FROM HPCSERVICE.EDI_DOC_DETAILS EDD
                    WHERE EDD.FAILED = 1";

        internal static string Sql_SelectMatches()
            => $@"SELECT rgm.CUSTOMER_GLN ""CustomerGln""
                    ,to_char(rgm.CUSTOMER_ARTICLE) ""CustomerGoodId""
                    ,rgm.ID_GOOD ""GoodId""
                    ,rbc.BAR_CODE ""BarCode""
                    ,(SELECT NAME FROM ABT.ref_goods WHERE ID = rgm.ID_GOOD) ""Name""
                    FROM ABT.REF_GOODS_MATCHING rgm, ABT.ref_bar_codes rbc
                        WHERE DISABLED = 0 AND rgm.ID_GOOD = rbc.ID_GOOD";

        internal static string Sql_SelectPriceType()
           => @"SELECT RPT.ID ""PriceId""
                ,RPT.NAME ""PriceName""
                ,RPT.COEF ""PriceCoef""
                ,RC.NAME  ""CurrencyName""
                ,(SELECT NAME FROM ABT.REF_PRICE_TYPES WHERE id = RPT.ID_PARENT_PRICE_TYPE) ""ParentName""
                FROM ABT.REF_PRICE_TYPES RPT, abt.REF_CURRENCIES RC
                WHERE rpt.ID_DEFAULT_CURRENCY = rc.ID AND RPT.ID_PARENT_PRICE_TYPE = 120401
                ";

        internal static string Sql_SelectMatchingPriceTypes()
            => $@"SELECT rptm.CUSTOMER_GLN ""CustomerGln""
                  ,rptm.ID_PRICE_TYPE ""IdPriceType""
                  ,rptm.DISABLED ""Disabled""
                  ,rptm.DISABLED_DATETIME ""DisabledDatetime""
                  ,rptm.INSERT_DATETIME ""InsertDatetime""
                  ,rptm.INSERT_USER ""InsertUser""
                  ,(SELECT name FROM abt.ref_price_types WHERE id = rptm.ID_PRICE_TYPE) ""PriceTypeName""
                FROM ABT.REF_PRICE_TYPES_MATCHING rptm
                WHERE DISABLED = 0";

    }
}
using LIMS.Assay.Base.Dto;
using LIMS.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base
{
    public interface IAssay_Formula
    {
        List<AssayEleFormula> GetFormulaByEleId(int input);
        List<AssayFormulaPram> GetPramsByFormulaId(int input);
        HtmlDataOperRetDto DeleteFormulaByFormulaId(int input);
        HtmlDataOperRetDto AddFormulaById(int input, CreateFormulaDto formulaInfo);
        HtmlDataOperRetDto UpdateFormulaById(int input, CreateFormulaDto formula);
        HtmlDataOperRetDto SetDefaultFormulaById(int input);

        HtmlDataOperRetDto DeleteConstById(int input);
        HtmlDataOperRetDto AddConst(CreateConstDto input);
        HtmlDataOperRetDto EditConst(CreateConstDto input,int inputId);
        List<CreateConstDto> GetAllConst();
        List<CreateConstDto> GetConstByEleId(int elementId);
    }
}

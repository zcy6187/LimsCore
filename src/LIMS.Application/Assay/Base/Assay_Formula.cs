﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using LIMS.Assay.Base.Dto;
using LIMS.Dtos;

namespace LIMS.Assay.Base
{
    public class Assay_Formula : LIMSAppServiceBase, IAssay_Formula
    {
        private IRepository<AssayEleFormula, int> _eleFormulaRep;
        private IRepository<AssayFormulaPram, int> _formulaPramRep;
        private IRepository<AssayConst, int> _constRep;

        public Assay_Formula(IRepository<AssayEleFormula, int> eleFormulaRep,
            IRepository<AssayFormulaPram, int> formulaPramRep,
            IRepository<AssayConst, int> constRep)
        {
            this._eleFormulaRep = eleFormulaRep;
            this._formulaPramRep = formulaPramRep;
            this._constRep = constRep;
        }

        // 公式
        public HtmlDataOperRetDto AddFormulaById(int input, CreateFormulaDto formula)
        {
            int rowCount = this._eleFormulaRep.GetAll().Where(x => x.eleId == input && x.name == formula.name).Count();
            if (rowCount > 0)
            {
                return new HtmlDataOperRetDto()
                {
                    Code = -1,
                    Message = "该名称已存在!"
                };
            }
            string formulaStr = formula.formulaExp;
            // 处理特殊字符，转小写
            formulaStr = formulaStr.Replace('）', ')');
            formulaStr = formulaStr.Replace('（', '(');
            formulaStr = formulaStr.ToLower();
            formulaStr = formulaStr.Replace(" ", "");

            // 解决mv/c这种匹配问题
            string expFromat = "([tmvc]\\d *){ 2,}";
            var expMatch = Regex.Matches(expFromat, formulaStr);
            if (expMatch.Count > 0)
            {
                return new HtmlDataOperRetDto()
                {
                    Code = -1,
                    Message = "该公式不合法!"
                };
            }

            // 简单的匹配
            string mPattern = "m[0-9]*";
            string vPattern = "v[0-9]*";
            string cPattern = "c[0-9]*";
            string tPattern = "t[0-9]*";

            // 参数替换成常数99，判断是否是正常的四则表达式
            string tempFormula = Regex.Replace(formulaStr, mPattern, "99");
            tempFormula = Regex.Replace(tempFormula, vPattern, "99");
            tempFormula = Regex.Replace(tempFormula, cPattern, "99");
            tempFormula = Regex.Replace(tempFormula, tPattern, "99");

            if (CheckExpressionValid(tempFormula))
            {
                // 提取参数
                List<string> pramList = new List<string>();
                var mMatches = Regex.Matches(formulaStr, mPattern);
                foreach (var item in mMatches)
                {
                    pramList.Add(item.ToString());
                }
                var cMatches = Regex.Matches(formulaStr, cPattern);
                foreach (var item in cMatches)
                {
                    pramList.Add(item.ToString());
                }
                var vMatch = Regex.Matches(formulaStr, vPattern);
                foreach (var item in vMatch)
                {
                    pramList.Add(item.ToString());
                }
                var tMatch = Regex.Matches(formulaStr, tPattern);
                foreach (var item in tMatch)
                {
                    pramList.Add(item.ToString());
                }


                AssayEleFormula tmpFormula = formula.MapTo<AssayEleFormula>();
                tmpFormula.formulaExp = formulaStr;
                tmpFormula.operatorId = AbpSession.UserId ?? 0;
                tmpFormula.IsDeleted = false;
                tmpFormula.lastModifyTime = DateTime.Now;
                tmpFormula.eleId = input;
                int tmpId = _eleFormulaRep.InsertAndGetId(tmpFormula);
                InserPrams(tmpId, pramList);

                return
                    new HtmlDataOperRetDto()
                    {
                        Code = 1,
                        Message = "添加成功！"
                    };
            }
            else
            {
                return
                    new HtmlDataOperRetDto()
                    {
                        Code = -1,
                        Message = "该公式不合法！"
                    };
            }
        }

        public HtmlDataOperRetDto UpdateFormulaById(int input, CreateFormulaDto formula)
        {
            var oldItem = _eleFormulaRep.Single(x => x.Id == input);
            if (oldItem == null)
            {
                return
                    new HtmlDataOperRetDto()
                    {
                        Code = -1,
                        Message = "该公式不存在！"
                    };
            }

            int rowCount = this._eleFormulaRep.GetAll().Where(x => x.eleId == oldItem.eleId && x.name == formula.name).Count();
            if (rowCount > 1)
            {
                return new HtmlDataOperRetDto()
                {
                    Code = -1,
                    Message = "该名称已存在!"
                };
            }

            string formulaStr = formula.formulaExp;
            // 处理特殊字符，转小写
            formulaStr = formulaStr.Replace('）', ')');
            formulaStr = formulaStr.Replace('（', '(');
            formulaStr = formulaStr.ToLower();
            formulaStr = formulaStr.Replace(" ", "");

            // 解决mv/c这种匹配问题
            string expFromat = "([tmvc]\\d *){ 2,}";
            var expMatch = Regex.Matches(expFromat, formulaStr);
            if (expMatch.Count > 0)
            {
                return new HtmlDataOperRetDto()
                {
                    Code = -1,
                    Message = "该公式不合法!"
                };
            }

            // 简单的匹配
            string mPattern = "m\\d*";
            string vPattern = "v\\d*";
            string cPattern = "c\\d*";
            string tPattern = "t\\d*";

            // 参数替换成常数99，判断是否是正常的四则表达式
            string tempFormula = Regex.Replace(formulaStr, mPattern, "99");
            tempFormula = Regex.Replace(tempFormula, vPattern, "99");
            tempFormula = Regex.Replace(tempFormula, cPattern, "99");
            tempFormula = Regex.Replace(tempFormula, tPattern, "99");

            if (CheckExpressionValid(tempFormula))
            {
                // 提取参数
                List<string> pramList = new List<string>();
                var mMatches = Regex.Matches(formulaStr, mPattern);
                foreach (var item in mMatches)
                {
                    pramList.Add(item.ToString());
                }
                var cMatches = Regex.Matches(formulaStr, cPattern);
                foreach (var item in cMatches)
                {
                    pramList.Add(item.ToString());
                }
                var vMatch = Regex.Matches(formulaStr, vPattern);
                foreach (var item in vMatch)
                {
                    pramList.Add(item.ToString());
                }
                var tMatch = Regex.Matches(formulaStr, tPattern);
                foreach (var item in tMatch)
                {
                    pramList.Add(item.ToString());
                }

                oldItem.name = formula.name;
                oldItem.formulaExp = formulaStr;
                oldItem.operatorId = AbpSession.UserId ?? 0;
                oldItem.lastModifyTime = DateTime.Now;
                _eleFormulaRep.Update(oldItem);

                int insertId = oldItem.Id;
                UpdatePrams(insertId, pramList);

                return
                    new HtmlDataOperRetDto()
                    {
                        Code = 1,
                        Message = "修改成功！"
                    };
            }
            else
            {
                return
                    new HtmlDataOperRetDto()
                    {
                        Code = -1,
                        Message = "公式不合法！"
                    };
            }


        }

        private void InserPrams(int formulaId, List<string> pramList)
        {
            foreach (var item in pramList)
            {
                AssayFormulaPram tmpPram = new AssayFormulaPram();
                tmpPram.formulaId = formulaId;
                tmpPram.pramName = item;
                _formulaPramRep.Insert(tmpPram);
            }
        }

        private void UpdatePrams(int formulaId, List<string> pramList)
        {
            var items = _formulaPramRep.GetAll().Where(x => x.formulaId == formulaId);
            foreach (var item in items)
            {
                _formulaPramRep.Delete(item);
            }

            foreach (var item in pramList)
            {
                AssayFormulaPram tmpPram = new AssayFormulaPram();
                tmpPram.formulaId = formulaId;
                tmpPram.pramName = item;
                _formulaPramRep.Insert(tmpPram);
            }
        }

        private bool CheckExpressionValid(string input)
        {
            string pattern = @"^(((?<o>\()[-+]?([0-9]+[-+*/])*)+[0-9]+((?<-o>\))([-+*/][0-9]+)*)+($|[-+*/]))*(?(o)(?!))$";
            //去掉空格，且添加括号便于进行匹配    
            return Regex.IsMatch("(" + input.Replace(" ", "") + ")", pattern);
        }

        public HtmlDataOperRetDto DeleteFormulaByFormulaId(int input)
        {
            var formulaItem = this._eleFormulaRep.Single(x => x.Id == input);
            formulaItem.IsDeleted = true;
            this._eleFormulaRep.Update(formulaItem);

            var pramItems = this._formulaPramRep.GetAll().Where(x => x.formulaId == input);
            foreach (var pram in pramItems)
            {
                this._formulaPramRep.Delete(pram);
            }

            return
                new HtmlDataOperRetDto()
                {
                    Code = 1,
                    Message = "操作成功！"
                };
        }

        // 将元素设为默认值
        public HtmlDataOperRetDto SetDefaultFormulaById(int input)
        {
            var formulaItem = this._eleFormulaRep.Single(x => x.Id == input);

            // 将先前默认元素设为非默认
            var allEleFormula = this._eleFormulaRep.GetAll().Where(x => x.eleId == formulaItem.eleId && x.flag==1).ToList();
            foreach (var item in allEleFormula)
            {
                item.flag = 0;
                this._eleFormulaRep.Update(item);
            }
            // 当前元素设为默认
            formulaItem.flag = 1;
            this._eleFormulaRep.Update(formulaItem);

            return
                new HtmlDataOperRetDto()
                {
                    Code = 1,
                    Message = "操作成功！"
                };
        }

        public List<AssayEleFormula> GetFormulaByEleId(int input)
        {
            var formulaList = this._eleFormulaRep.GetAll().Where(x => x.eleId == input).ToList();
            return formulaList;
        }

        public List<AssayFormulaPram> GetPramsByFormulaId(int input)
        {
            var pramList = this._formulaPramRep.GetAll().Where(x => x.formulaId == input).ToList();
            return pramList;
        }


        // 常数
        public HtmlDataOperRetDto DeleteConstById(int input)
        {
            var constItem = this._constRep.Single(x => x.Id == input);
            constItem.IsDeleted = true;
            this._constRep.Update(constItem);
            return new HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "操作成功！"
            };
        }
        public HtmlDataOperRetDto AddConst(CreateConstDto input)
        {
            AssayConst item = new AssayConst();
            item.constVal = input.constVal;
            item.elementId = input.elementId;
            item.intro = input.intro;
            item.operatorId = AbpSession.UserId ?? 0;
            item.lastModifyTime = DateTime.Now;

            this._constRep.Insert(item);
            return new HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "操作成功！"
            };

        }
        public HtmlDataOperRetDto EditConst(CreateConstDto input, int inputId)
        {
            var constItem = this._constRep.Single(x => x.Id == inputId);
            constItem.intro = input.intro;
            constItem.operatorId = AbpSession.UserId ?? 0;
            constItem.constVal = input.constVal;
            constItem.elementId = input.elementId;
            constItem.lastModifyTime = DateTime.Now;

            this._constRep.Update(constItem);
            return new HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "操作成功！"
            };
        }
        public List<CreateConstDto> GetAllConst()
        {
            var constItems = this._constRep.GetAll().Select(x => new CreateConstDto()
            {
                id = x.Id,
                constVal = x.constVal,
                elementId = x.elementId,
                intro = x.intro,
                operatorId = x.operatorId
            }).OrderByDescending(x=>x.id).ToList();

            return constItems;
        }
        // 获取单个元素的所有ID
        public List<CreateConstDto> GetConstByEleId(int elementId)
        {
            var constItems = this._constRep.GetAll().Where(x=>x.elementId==elementId).Select(x => new CreateConstDto()
            {
                id = x.Id,
                constVal = x.constVal,
                elementId = x.elementId,
                intro = x.intro,
                operatorId = x.operatorId
            }).OrderByDescending(x => x.id).ToList();

            return constItems;
        }
        // 获取单个常数的信息
        public CreateConstDto GetConstByConstId(int constId)
        {
            var constItem = this._constRep.GetAll().Where(x => x.Id == constId).Select(x => new CreateConstDto()
            {
                id = x.Id,
                constVal = x.constVal,
                elementId = x.elementId,
                intro = x.intro,
                operatorId = x.operatorId
            }).FirstOrDefault();

            return constItem;
        }
    }
}

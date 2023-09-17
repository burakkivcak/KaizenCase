using KaizenCase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KaizenCase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeCaseController : Controller
    {
        public readonly string codeCharacters = "ACDEFGHKLMNPRTXYZ234579";

        [HttpGet]
        [Route("CreateCode")]
        public IActionResult CreateCode()
        {
            List<string> codeList = new List<string>();
            List<CodeLineUp> codeLineUps = new List<CodeLineUp>();
            var codeCharacterArray = codeCharacters.Select(x => x.ToString()).ToArray();
            Random randomArrayNumber = new Random();
            Random randomCharacterNumber = new Random();

            for (int i = 0; i < 1000; i++)
            {
                int arrayNumber1 = randomArrayNumber.Next(3, 6);
                int characterNumber1 = randomCharacterNumber.Next(codeCharacterArray.Count() - 1);
                int arrayNumber2 = randomArrayNumber.Next(3, 6);
                while (arrayNumber1 == arrayNumber2)
                {
                    arrayNumber2 = randomArrayNumber.Next(3, 6);
                }
                int characterNumber2 = randomCharacterNumber.Next(codeCharacterArray.Count() - 1);
                bool isUpperItteration1 = characterNumber1 > (codeCharacterArray.Count() / 2) ? false : true;
                bool isUpperItteration2 = characterNumber2 > (codeCharacterArray.Count() / 2) ? false : true;
                if (codeLineUps.Any(x => x.ArrayNumber1 == arrayNumber1 && x.ArrayNumber2 == arrayNumber2
                        && x.CharacterNumber1 == characterNumber1 && x.CharacterNumber2 == characterNumber2))
                {
                    i--;
                    continue;
                }
                else
                {
                    codeLineUps.Add(new CodeLineUp()
                    {
                        ArrayNumber1 = arrayNumber1,
                        CharacterNumber1 = characterNumber1,
                        ArrayNumber2 = arrayNumber2,
                        CharacterNumber2 = characterNumber2
                    });
                }
                string[] newCodeArray = { "", "", "", "", "", "", "", "" };
                newCodeArray[7] = codeCharacterArray[characterNumber1];
                newCodeArray[1] = codeCharacterArray[characterNumber2];
                if (isUpperItteration1)
                {
                    var firstDataIndex = characterNumber1 + arrayNumber1;
                    newCodeArray[0] = codeCharacterArray[firstDataIndex];

                }
                else
                {
                    var firstDataIndex = characterNumber1 - arrayNumber1;
                    newCodeArray[0] = codeCharacterArray[firstDataIndex];
                }
                if (isUpperItteration2)
                {
                    var seventhDataIndex = characterNumber2 + arrayNumber2;
                    newCodeArray[6] = codeCharacterArray[seventhDataIndex];
                }
                else
                {
                    var seventhDataIndex = characterNumber2 - arrayNumber2;
                    newCodeArray[6] = codeCharacterArray[seventhDataIndex];
                }
                var modIndex1 = (arrayNumber1 * characterNumber1) % codeCharacterArray.Count();
                var modIndex2 = (arrayNumber2 * characterNumber2) % codeCharacterArray.Count();
                var modIndex3 = (arrayNumber1 * characterNumber2) % codeCharacterArray.Count();
                var modIndex4 = (arrayNumber2 * characterNumber1) % codeCharacterArray.Count();
                newCodeArray[arrayNumber1 - 1] = codeCharacterArray[modIndex1];
                newCodeArray[arrayNumber2 - 1] = codeCharacterArray[modIndex2];
                newCodeArray[Array.FindIndex(newCodeArray, w => string.IsNullOrEmpty(w))] = codeCharacterArray[modIndex3];
                newCodeArray[Array.FindIndex(newCodeArray, w => string.IsNullOrEmpty(w))] = codeCharacterArray[modIndex4];
                var code = string.Join("", newCodeArray);
                codeList.Add(code);
            }
            return Ok(codeList);
        }
        [HttpPost]
        [Route("CheckCode")]
        public IActionResult CheckCode([FromBody]string code)
        {
            if(string.IsNullOrEmpty(code))
            {
                return Ok(ReturnCodeEnum.EmptyCode);
            }
            var codeCharacterArray = codeCharacters.Select(x => x.ToString().ToLower()).ToArray();
            var codeArray = code.Select(x => x.ToString().ToLower()).ToArray();
            if(codeArray.Count() != 8)
            {
                return Ok(ReturnCodeEnum.WrongSizeCode);
            }
            for (int i = 0; i < codeArray.Count(); i++)
            {
                if(codeCharacterArray.Any(x=> x.Contains(codeArray[i])))
                {
                    continue;
                }
                else
                {
                    return Ok(ReturnCodeEnum.WrongCode);
                }
            }
            string[] checkCodeArray = { "", "", "", "", "", "", "", "" };
            try
            {
                int characterNumber1 = Array.FindIndex(codeCharacterArray, w => w == codeArray[7]);
                int characterNumber2 = Array.FindIndex(codeCharacterArray, w => w == codeArray[1]);
                int arrayCharacterNumber1 = Array.FindIndex(codeCharacterArray, w => w == codeArray[0]);
                int arrayCharacterNumber2 = Array.FindIndex(codeCharacterArray, w => w == codeArray[6]);
                bool isUpperItteration1 = characterNumber1 > (codeCharacterArray.Count() / 2) ? false : true;
                bool isUpperItteration2 = characterNumber2 > (codeCharacterArray.Count() / 2) ? false : true;
                int arrayNumber1 = isUpperItteration1 ? arrayCharacterNumber1 - characterNumber1 : characterNumber1 - arrayCharacterNumber1;
                int arrayNumber2 = isUpperItteration2 ? arrayCharacterNumber2 - characterNumber2 : characterNumber2 - arrayCharacterNumber2;
                var modIndex1 = (arrayNumber1 * characterNumber1) % codeCharacterArray.Count();
                var modIndex2 = (arrayNumber2 * characterNumber2) % codeCharacterArray.Count();
                var modIndex3 = (arrayNumber1 * characterNumber2) % codeCharacterArray.Count();
                var modIndex4 = (arrayNumber2 * characterNumber1) % codeCharacterArray.Count();
                checkCodeArray[0] = codeArray[0];
                checkCodeArray[1] = codeArray[1];
                checkCodeArray[6] = codeArray[6];
                checkCodeArray[7] = codeArray[7];
                if (codeArray[arrayNumber1 - 1] != codeCharacterArray[modIndex1])
                {
                    return Ok(ReturnCodeEnum.WrongCode);
                }
                else
                {
                    checkCodeArray[arrayNumber1 - 1] = codeArray[arrayNumber1 - 1];
                }
                if(codeArray[arrayNumber2 - 1] != codeCharacterArray[modIndex2])
                {
                    return Ok(ReturnCodeEnum.WrongCode);
                }
                else
                {
                    checkCodeArray[arrayNumber2 - 1] = codeArray[arrayNumber2 - 1];
                }
                if(codeArray[Array.FindIndex(checkCodeArray, w => string.IsNullOrEmpty(w))] != codeCharacterArray[modIndex3])
                {
                    return Ok(ReturnCodeEnum.WrongCode);
                }
                else
                {
                    checkCodeArray[Array.FindIndex(checkCodeArray, w => string.IsNullOrEmpty(w))] = codeCharacterArray[modIndex3];
                }
                if(codeArray[Array.FindIndex(checkCodeArray, w => string.IsNullOrEmpty(w))] != codeCharacterArray[modIndex4])
                {
                    return Ok(ReturnCodeEnum.WrongCode);
                }
                else
                {
                    checkCodeArray[Array.FindIndex(checkCodeArray, w => string.IsNullOrEmpty(w))] = codeCharacterArray[modIndex4];
                }
                var checkCode = string.Join("", checkCodeArray);
                if(checkCode == code.ToLower())
                {
                    return Ok(ReturnCodeEnum.Success);
                }
                else
                {
                    return Ok(ReturnCodeEnum.WrongCode);
                }
            }
            catch (Exception ex)
            {
                return Ok(ReturnCodeEnum.WrongCode);
            }
        }
    }
}

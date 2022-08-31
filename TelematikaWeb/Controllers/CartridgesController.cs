using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TelematikaWEB.Models;
using TelematikaWeb.Data;
using System.Diagnostics;

namespace TelematikaWeb.Controllers
{
    public class CartridgesController : Controller
    {
        private readonly TelematikaWebContext _context;
        public List<Cartridge> dataCartridge = new List<Cartridge>();
        public List<Cartridge> availableCartridge = new List<Cartridge>();
        public List<Cartridge> resultCartridge = new List<Cartridge>();

        public CartridgesController(TelematikaWebContext context)
        {
            _context = context;
        }

        // GET: Cartridges
        public async Task<IActionResult> Index()
        {
              return _context.Cartridge != null ? 
                          View(await _context.Cartridge.ToListAsync()) :
                          Problem("Entity set 'TelematikaWebContext.Cartridge'  is null.");
        }

        // GET: Cartridges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cartridge == null)
            {
                return NotFound();
            }

            var cartridge = await _context.Cartridge
                .FirstOrDefaultAsync(m => m.id == id);
            if (cartridge == null)
            {
                return NotFound();
            }

            return View(cartridge);
        }

        // GET: Cartridges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cartridges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id, number,value,count,isAvailable")] Cartridge cartridge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartridge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cartridge);
        }

        // GET: Cartridges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cartridge == null)
            {
                return NotFound();
            }

            var cartridge = await _context.Cartridge.FindAsync(id);
            if (cartridge == null)
            {
                return NotFound();
            }
            return View(cartridge);
        }

        // POST: Cartridges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,number,value,count,isAvailable")] Cartridge cartridge)
        {
            if (id != cartridge.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartridge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartridgeExists(cartridge.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cartridge);
        }

        // GET: Cartridges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cartridge == null)
            {
                return NotFound();
            }

            var cartridge = await _context.Cartridge
                .FirstOrDefaultAsync(m => m.id == id);
            if (cartridge == null)
            {
                return NotFound();
            }

            return View(cartridge);
        }

        // POST: Cartridges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cartridge == null)
            {
                return Problem("Entity set 'TelematikaWebContext.Cartridge'  is null.");
            }
            var cartridge = await _context.Cartridge.FindAsync(id);
            if (cartridge != null)
            {
                _context.Cartridge.Remove(cartridge);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartridgeExists(int id)
        {
          return (_context.Cartridge?.Any(e => e.id == id)).GetValueOrDefault();
        }
        //end of auto-generation

        public IActionResult PrintAvailableCartridge()
        {
            dataCartridge.AddRange(_context.Cartridge);
            for (int i = 0; i < dataCartridge.Count; i++)
            {
                if (dataCartridge[i].isAvailable==true) 
                {
                    availableCartridge.Add(dataCartridge[i]);
                }
            }
            IEnumerable<Cartridge> query = availableCartridge.OrderByDescending(с => с.value).ToList();
            availableCartridge = (List<Cartridge>)query;
            return View(availableCartridge);
        }

        [HttpGet]
        public IActionResult Issuance()
        {
            PrintAvailableCartridge();
            return View();
        }
        private bool Solve(int ind, int remaind)
        {
            if (ind > availableCartridge.Count - 1)
            {
                return false;
            }
            int remainderCount;
            int remainder;

            remainderCount = remaind / availableCartridge[ind].value;
            if (remainderCount == 0)
            {
                if (ind < availableCartridge.Count - 1)
                {
                    return Solve(ind + 1, remaind);
                }
                else
                {
                    return false;
                }
            }
            if (remainderCount > availableCartridge[ind].count)
            {
                remainderCount = availableCartridge[ind].count;
            }
            resultCartridge.Add(new Cartridge()
            {
                number = availableCartridge[ind].number,
                value = availableCartridge[ind].value,
                count = remainderCount,
                isAvailable = true
            });
            remainder = remaind - remainderCount * availableCartridge[ind].value;
            if (remainder == 0)
            {
                return true;
            }
            else
            {
                if (ind < availableCartridge.Count - 1)
                {
                    if (Solve(ind + 1, remainder))
                    {
                        return true;
                    }
                    else
                    {
                        resultCartridge[resultCartridge.Count - 1].count = resultCartridge[resultCartridge.Count - 1].count - 1;
                        if (resultCartridge[resultCartridge.Count - 1].count == 0)
                        {
                            resultCartridge.Remove(resultCartridge[resultCartridge.Count - 1]);
                        }
                        remaind = remainder + availableCartridge[ind].value;
                        return Solve(ind + 1, remaind);
                    }
                }
                else
                {
                    resultCartridge.Remove(resultCartridge[resultCartridge.Count - 1]);
                    return false;
                }
            }
        }
        [HttpPost]
        public IActionResult Issuance(string money)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string moneyData = $"Вы ввели: {money}"+'\n';
            PrintAvailableCartridge();
            int allMoney = 0;
            
            for (int i = 0; i < availableCartridge.Count; i++)
            {
                allMoney += availableCartridge[i].value * availableCartridge[i].count;
            }
            if (int.TryParse(money, out int parseMoney))
            {
                if (parseMoney > allMoney)
                {
                    moneyData += "недостаточно денег" + '\n';
                }
                if (!Solve(0, parseMoney))
                {
                    moneyData += "не получилось выдать" + '\n';
                }
                //вывод картриджа
                else {moneyData += ListTostring(resultCartridge); }
            }
            else
            {
                moneyData += "не удалось преобразовать" + '\n';
            }
            stopwatch.Stop();
            moneyData += $"время выполнения (milliseconds): {stopwatch.ElapsedMilliseconds}\n";
            return Content(moneyData);
        }

        private string ListTostring(List<Cartridge> availableCartridge)
        {
            string str = "";
            foreach (Cartridge cartridge in availableCartridge)
            {
                str += $"взять из картриджа №{ cartridge.number.ToString()} {cartridge.value.ToString()} в количестве: {cartridge.count.ToString()}" + '\n';
            }
            return str;
        }
    }
}

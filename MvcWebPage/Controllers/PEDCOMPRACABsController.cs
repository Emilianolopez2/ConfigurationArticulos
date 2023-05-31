using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcWebPage.MLAVID;

namespace MvcWebPage.Controllers
{
    public class PEDCOMPRACABsController : Controller
    {
        private readonly MLAVIDContext _context;

        public PEDCOMPRACABsController(MLAVIDContext context)
        {
            _context = context;
        }

        // GET: PEDCOMPRACABs
        public async Task<IActionResult> Index()
        {
              return View(await _context.PEDCOMPRACAB.ToListAsync());
        }

        // GET: PEDCOMPRACABs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.PEDCOMPRACAB == null)
            {
                return NotFound();
            }

            var pEDCOMPRACAB = await _context.PEDCOMPRACAB
                .FirstOrDefaultAsync(m => m.NUMSERIE == id);
            if (pEDCOMPRACAB == null)
            {
                return NotFound();
            }

            return View(pEDCOMPRACAB);
        }

        // GET: PEDCOMPRACABs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PEDCOMPRACABs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NUMSERIE,NUMPEDIDO,N,CODPROVEEDOR,SERIEALBARAN,NUMEROALBARAN,NALBARAN,FECHAPEDIDO,FECHAENTREGA,ENVIOPOR,TOTBRUTO,DTOPP,TOTDTOPP,DTOCOMERCIAL,TOTDTOCOMERCIAL,TOTIMPUESTOS,TOTNETO,CODMONEDA,FACTORMONEDA,PORTESPAG,SUPEDIDO,IVAINCLUIDO,TODORECIBIDO,TIPODOC,IDESTADO,FECHAMODIFICADO,HORA,TRANSPORTE,NBULTOS,TOTALCARGOSDTOS,NORECIBIDO,CODEMPLEADO,CONTACTO,FROMPEDVENTACENTRAL,FECHACREACION,NUMIMPRESIONES,REGIMFACT")] PEDCOMPRACAB pEDCOMPRACAB)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pEDCOMPRACAB);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pEDCOMPRACAB);
        }

        // GET: PEDCOMPRACABs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.PEDCOMPRACAB == null)
            {
                return NotFound();
            }

            var pEDCOMPRACAB = await _context.PEDCOMPRACAB.FindAsync(id);
            if (pEDCOMPRACAB == null)
            {
                return NotFound();
            }
            return View(pEDCOMPRACAB);
        }

        // POST: PEDCOMPRACABs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NUMSERIE,NUMPEDIDO,N,CODPROVEEDOR,SERIEALBARAN,NUMEROALBARAN,NALBARAN,FECHAPEDIDO,FECHAENTREGA,ENVIOPOR,TOTBRUTO,DTOPP,TOTDTOPP,DTOCOMERCIAL,TOTDTOCOMERCIAL,TOTIMPUESTOS,TOTNETO,CODMONEDA,FACTORMONEDA,PORTESPAG,SUPEDIDO,IVAINCLUIDO,TODORECIBIDO,TIPODOC,IDESTADO,FECHAMODIFICADO,HORA,TRANSPORTE,NBULTOS,TOTALCARGOSDTOS,NORECIBIDO,CODEMPLEADO,CONTACTO,FROMPEDVENTACENTRAL,FECHACREACION,NUMIMPRESIONES,REGIMFACT")] PEDCOMPRACAB pEDCOMPRACAB)
        {
            if (id != pEDCOMPRACAB.NUMSERIE)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pEDCOMPRACAB);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PEDCOMPRACABExists(pEDCOMPRACAB.NUMSERIE))
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
            return View(pEDCOMPRACAB);
        }

        // GET: PEDCOMPRACABs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.PEDCOMPRACAB == null)
            {
                return NotFound();
            }

            var pEDCOMPRACAB = await _context.PEDCOMPRACAB
                .FirstOrDefaultAsync(m => m.NUMSERIE == id);
            if (pEDCOMPRACAB == null)
            {
                return NotFound();
            }

            return View(pEDCOMPRACAB);
        }

        // POST: PEDCOMPRACABs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.PEDCOMPRACAB == null)
            {
                return Problem("Entity set 'MLAVIDContext.PEDCOMPRACAB'  is null.");
            }
            var pEDCOMPRACAB = await _context.PEDCOMPRACAB.FindAsync(id);
            if (pEDCOMPRACAB != null)
            {
                _context.PEDCOMPRACAB.Remove(pEDCOMPRACAB);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PEDCOMPRACABExists(string id)
        {
          return _context.PEDCOMPRACAB.Any(e => e.NUMSERIE == id);
        }
    }
}

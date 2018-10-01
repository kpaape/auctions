using Microsoft.EntityFrameworkCore;
using FinalExam.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
namespace FinalExam.Controllers
{
    public class FinalController : Controller
    {
        private FinalContext dbContext;
    
        public FinalController(FinalContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            if(ViewBag.UserId == null)
            {
                return RedirectToAction("Index", "Logins");
            }
            ViewBag.Wallet = dbContext.Users.Where(u => u.user_id == HttpContext.Session.GetInt32("UserId")).First().wallet;

            var AllAuctions = dbContext.Auctions
                .Include(auction => auction.creator)
                .OrderByDescending(auction => auction.created_at).ToList();

            // this will process auctions
            foreach(var auction in AllAuctions)
            {
                if((int)(DateTime.Now - auction.created_at).TotalDays < 0)
                {
                    var bidder = dbContext.Users.FirstOrDefault(u => u.user_id == auction.highest_bidder_id);
                    auction.creator.wallet += auction.highest_bid;
                    bidder.wallet -= auction.highest_bid;
                    dbContext.Auctions.Remove(auction);
                    dbContext.SaveChanges();
                }
            }
            // auctions have been processed, grab remaining

            AllAuctions = dbContext.Auctions
                .Include(auction => auction.creator)
                .OrderByDescending(auction => auction.created_at).ToList();
            return View(AllAuctions);
        }

        [HttpGet("CreateAuction")]
        public IActionResult CreateAuction()
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            if(ViewBag.UserId == null)
            {
                return RedirectToAction("Index", "Logins");
            }

            return View("CreateAuction");
        }

        [HttpPost("AddAuction")]
        public IActionResult AddAuction(Auction NewAuction)
        {
            if(ModelState.IsValid)
            {
                int? UserId = HttpContext.Session.GetInt32("UserId");
                if(UserId == null)
                {
                    return View("Index", "Logins");
                }
                NewAuction.user_id = (int)UserId;
                dbContext.Auctions.Add(NewAuction);
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("CreateAuction");
        }

        [HttpGet("DeleteAuction/{ToDelete}")]
        public IActionResult DeleteAuction(int ToDelete)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = UserId;
            if(ViewBag.UserId == null)
            {
                return RedirectToAction("Index", "Logins");
            }
            var DelAuction = dbContext.Auctions.FirstOrDefault(a => a.auction_id == ToDelete);
            if(DelAuction.user_id == UserId)
            {
                dbContext.Auctions.Remove(DelAuction);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet("ViewAuction/{ToView}")]
        public IActionResult ViewAuction(int ToView)
        {
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            if(ViewBag.UserId == null)
            {
                return RedirectToAction("Index", "Logins");
            }
            HttpContext.Session.SetInt32("Viewing", ToView);
            var ViewAuction = dbContext.Auctions
                .Include(auction => auction.creator)
                .FirstOrDefault(auction => auction.auction_id == ToView);
            ViewBag.HighestBidder = dbContext.Users.FirstOrDefault(u => u.user_id == ViewAuction.highest_bidder_id);
            ViewBag.ViewAuction = ViewAuction;
            return View("ViewAuction");
        }

        [HttpPost("SubmitBid")]
        public IActionResult SubmitBid(Bid NewBid)
        {
            int? Viewing = HttpContext.Session.GetInt32("Viewing");
            if(ModelState.IsValid)
            {
                int? UserId =HttpContext.Session.GetInt32("UserId");
                ViewBag.UserId = UserId;
                if(ViewBag.UserId == null)
                {
                    return RedirectToAction("Index", "Logins");
                }
                var ViewAuction = dbContext.Auctions
                    .Include(auction => auction.creator)
                    .FirstOrDefault(auction => auction.auction_id == (int)Viewing);
                decimal UserWallet = dbContext.Users.FirstOrDefault(u => u.user_id == UserId).wallet;
                if(NewBid.amount > ViewAuction.highest_bid && NewBid.amount < UserWallet)
                {
                    ViewAuction.highest_bid = NewBid.amount;
                    ViewAuction.highest_bidder_id = (int)HttpContext.Session.GetInt32("UserId");
                    dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("ViewAuction", new { ToView = Viewing });
        }
    }
}
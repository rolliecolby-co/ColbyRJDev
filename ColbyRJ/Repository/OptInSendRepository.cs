using MimeKit;

namespace ColbyRJ.Repository
{
    public class OptInSendRepository : IOptInSendRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _ctxFactory;
        private readonly IEmailService _emailService;
        private readonly int days = -8;
        private bool sendNow = false;

        public OptInSendRepository(
            IDbContextFactory<ApplicationDbContext> ctxFactory,
            IEmailService emailService)
        {
            _ctxFactory = ctxFactory;
            _emailService = emailService;
        }

        public async Task<string> GetChanges()
        {
            var htmlMsg = "";

            htmlMsg += await GetWeddings();
            htmlMsg += await GetBirthdays();
            htmlMsg += await GetAddressChanges();
            htmlMsg += await GetDocChanges();
            htmlMsg += await GetJobChanges();
            htmlMsg += await GetPhotoAlbumChanges();
            htmlMsg += await GetPhotoFolderChanges();
            htmlMsg += await GetPhotoGalleryChanges();
            htmlMsg += await GetSnippetChanges();
            htmlMsg += await GetStoryChanges();
            htmlMsg += await GetTripChanges();
            htmlMsg += await GetUnitChanges();
            htmlMsg += await GetVideoChanges();
            htmlMsg += await GetWhoAmIChanges();

            return htmlMsg;
        }

        public async Task<ICollection<OptInSendToDTO>> GetOptInEmails()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var deleteEmails = ctx.WorkEmails;
            ctx.WorkEmails.RemoveRange(deleteEmails);
            ctx.SaveChanges();

            List<OptInSendToDTO> sendToDTO = new List<OptInSendToDTO>();

            var emails = await ctx.AppUsers
                    .Where(a => a.InfoOptIn == true)
                    .ToListAsync();

            foreach (var item in emails)
            {
                try
                {
                    var workEmail = new WorkEmail();
                    workEmail.Email = item.Email;

                    await ctx.WorkEmails.AddAsync(workEmail);
                    await ctx.SaveChangesAsync();
                }
                catch { }
            }

            var emails3 = await ctx.WorkEmails
                .OrderBy(a => a.Email)
                .AsNoTracking()
                .ToListAsync();

            foreach (var item in emails3)
            {
                sendToDTO.Add(new OptInSendToDTO
                {
                    SendToEmail = item.Email
                });
            }

            return sendToDTO;
        }

        public async Task<ICollection<OptInSendToDTO>> GetOptInSendTo()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var optInEmails = ctx.WorkOptIn
              .Where(q => q.Sent == false)
              .AsNoTracking().ToList();

            List<OptInSendToDTO> sendToDTO = new List<OptInSendToDTO>();
            var sendTos = "";
            var emailCnt = 0;

            foreach (var item in optInEmails)
            {
                emailCnt++;
                sendTos += item.Email + " ";

                if (emailCnt == 9)
                {
                    sendToDTO.Add(new OptInSendToDTO
                    {
                        SendToEmail = sendTos
                    });

                    emailCnt = 0;
                    sendTos = "";
                }
            }

            if (sendTos.Length > 1)
            {
                sendToDTO.Add(new OptInSendToDTO
                {
                    SendToEmail = sendTos
                });
            }

            return sendToDTO;
        }

        public async Task<string> GetSatStatus()
        {
            var changesMsg = "";

            changesMsg += await GetChanges();

            changesMsg = "<h3>What's new on the Colby Family website (ColbyRJ.us)</h3>" + changesMsg;

            return changesMsg;
        }

        public async Task<string> SendOptIn()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var dateNow = DateTime.Now;
            var dateToday = DateTime.Today;
            var dateDOW = dateNow.ToString("ddd");

            var changesMsg = "";

            if (dateDOW == "Sat")
            {
                changesMsg += await GetChanges();

                if (changesMsg.Length > 25)
                {
                    await RefreshWorkOptIn(dateToday);
                }

                if (sendNow)
                {
                    changesMsg = "<h3>What's new on the Colby Family Memories website (ColbyRJ.us)</h3>" + changesMsg;

                    var users = await ctx.AppUsers
                            .Where(a => a.InfoOptIn == true)
                            .AsNoTracking()
                            .ToListAsync();

                    foreach (var user in users)
                    {
                        SendSatEmail(user.Email, user.DisplayName, changesMsg);
                    }
                }
            }

            return "ok";
        }

        public async Task RefreshWorkOptIn(DateTime dateToday)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var workOptIn = await ctx.WorkOptIn.FirstOrDefaultAsync();

            if (workOptIn == null)
            {
                WorkOptIn wOptIn = new WorkOptIn();

                wOptIn.RefreshDate = dateToday;

                await ctx.WorkOptIn.AddAsync(wOptIn);

                await ctx.SaveChangesAsync();

                sendNow = true;
            }
            else if (workOptIn.RefreshDate < dateToday)
            {
                workOptIn.RefreshDate = dateToday;

                await ctx.SaveChangesAsync();

                sendNow = true;
            }
        }

        public async void SendSatEmail(string sendToEmail, string sendToName, string changesMsg)
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var mailMessage = new MimeMessage();

            mailMessage.Subject = "Colby Family Memories weekly status";

            mailMessage.To.Add(new MailboxAddress(sendToName, sendToEmail));

            //mailMessage.Bcc.Add(new MailboxAddress("", "rollie@colbyrj.us"));
            //changesMsg += "<br />" + sendTo;

            var builder = new BodyBuilder();
            builder.HtmlBody = string.Format(changesMsg);
            mailMessage.Body = builder.ToMessageBody();

            await _emailService.Send(mailMessage);
        }

        public async Task<string> GetAddressChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesA = await ctx.Addresses
                .Where(q => q.DateUpdated > DateTime.Today.AddDays(days))
                .AsNoTracking().ToListAsync();

            var changesAC = await ctx.AddressComments
                .Include(ac => ac.Address)
                .Where(q => q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new { q.Address.Who, q.Address.HomeAddress })
                .Distinct()
                .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesA)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.Who,
                    ChangedTitle = item.HomeAddress
                });
            }

            foreach (var item in changesAC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.Who,
                    ChangedTitle = item.HomeAddress + " - Comment(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Home Addresses</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Who</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Address</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetBirthdays()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var users = await ctx.AppUsers
                .Where(q => q.DOB != null)
              .AsNoTracking().ToListAsync();

            users.ForEach(q =>
            {
                try
                {
                    var bdDate = Convert.ToDateTime(q.DOB);
                    var bdMon = Convert.ToDateTime(q.DOB).Month;
                    var currentYr = DateTime.Now.Year;
                    if (bdMon == 1)
                    {
                        currentYr++;
                    }
                    var strBdDate = bdDate.Month.ToString() + "/" + bdDate.Day.ToString() + "/" + currentYr.ToString();
                    q.Birthday = Convert.ToDateTime(strBdDate);
                }
                catch { }
            });

            users = users
                .Where(q => q.Birthday > DateTime.Today.AddDays(-1) && q.Birthday < DateTime.Today.AddDays(15))
                .ToList();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in users)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = -1,
                    WhatChanged = "Birthdays",
                    ChangedTitle = item.DisplayName + " - " + Convert.ToDateTime(item.Birthday).ToString("M/d/yyyy"),
                });
            }

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;' >Birthdays</th></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>Happy Birthday</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetDocChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesD = await ctx.Docs
                .Where(q => q.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            var changesDP = await ctx.DocPdfs
                .Include(dp => dp.Doc)
                .Where(q => q.Doc.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new { q.Doc.GroupedBy, q.Doc.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesDC = await ctx.DocComments
                .Include(dc => dc.Doc)
                .Where(q => q.Doc.Active == true && q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new { q.Doc.GroupedBy, q.Doc.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesD)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title
                });
            }

            foreach (var item in changesDP)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - PDF(s)"
                });
            }

            foreach (var item in changesDC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Comment(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Documents</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Grouped By</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Title</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetJobChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesJ = await ctx.Jobs
                .Where(q => q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            var changesJC = await ctx.JobComments
                .Include(jc => jc.Job)
                .Where(q => q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new { q.Job.Who, q.Job.Employer, q.Job.Position })
                .Distinct()
                .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesJ)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.Who,
                    ChangedTitle = item.Employer + " - " + item.Position
                });
            }

            foreach (var item in changesJC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.Who,
                    ChangedTitle = item.Employer + " - " + item.Position + " - Comment(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Jobs</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Who</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Employer - Position</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetPhotoAlbumChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesPA = await ctx.PhotoAlbums
                .Where(q => q.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            var changesPAP = await ctx.PhotoAlbumPhotos
                .Include(ap => ap.PhotoAlbum)
                .Where(q => q.PhotoAlbum.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new { q.PhotoAlbum.GroupedBy, q.PhotoAlbum.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesPAC = await ctx.PhotoAlbumComments
                .Include(sp => sp.PhotoAlbum)
                .Where(q => q.PhotoAlbum.Active == true && q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new { q.PhotoAlbum.GroupedBy, q.PhotoAlbum.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesPA)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title
                });
            }

            foreach (var item in changesPAP)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Photo(s)"
                });
            }

            foreach (var item in changesPAC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Comment(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: lightgray; text-align: center;'>Photo Albums</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Grouped By</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Title</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetPhotoFolderChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesPF = await ctx.PhotoFolders
                .Where(q => q.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            var changesPFC = await ctx.PhotoFolderComments
                .Include(sp => sp.PhotoFolder)
                .Where(q => q.PhotoFolder.Active == true && q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new { q.PhotoFolder.GroupedBy, q.PhotoFolder.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesPF)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title
                });
            }

            foreach (var item in changesPFC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Comment(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Photo Folders</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Grouped By</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Title</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetPhotoGalleryChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesGP = await ctx.GalleryPhotos
                .Include(q => q.Section)
                .Where(q => q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesGP)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.Section.Section,
                    ChangedTitle = item.Caption
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Gallery Photos</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Gallery Section</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Photo Caption</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetSnippetChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesS = await ctx.Snippets
                .Where(q => q.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            var changesSP = await ctx.SnippetPhotos
                .Include(sp => sp.Snippet)
                .Where(q => q.Snippet.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new { q.Snippet.GroupedBy, q.Snippet.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesSC = await ctx.SnippetComments
                .Include(sp => sp.Snippet)
                .Where(q => q.Snippet.Active == true && q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new { q.Snippet.GroupedBy, q.Snippet.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesS)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title
                });
            }

            foreach (var item in changesSP)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Photo(s)"
                });
            }

            foreach (var item in changesSC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Comment(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Snippets</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Grouped By</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Title</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetStoryChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesS = await ctx.Stories
                .Where(q => q.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            var changesSC = await ctx.StoryComments
                .Include(sc => sc.Story)
                .Where(q => q.Story.Active == true && q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new { q.Story.GroupedBy, q.Story.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesSP = await ctx.StoryPhotos
                .Include(sp => sp.Story)
                .Where(q => q.Story.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new { q.Story.GroupedBy, q.Story.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesSCp = await ctx.StoryChapters
                .Include(sc => sc.Story)
                .Where(q => q.Story.Active == true && q.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new { q.Story.GroupedBy, q.Story.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesSCpP = await ctx.StoryChapterPhotos
                .Include(q => q.Chapter)
                .ThenInclude(q => q.Story)
                .Where(q => q.Chapter.Story.Active == true && q.Chapter.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new { q.Chapter.Story.GroupedBy, q.Chapter.Story.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesS)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title
                });
            }

            foreach (var item in changesSC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Comment(s)"
                });
            }

            foreach (var item in changesSCp)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Chapter(s)"
                });
            }

            foreach (var item in changesSCpP)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Chapter Photo(s)"
                });
            }

            foreach (var item in changesSP)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Photo(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Stories</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Grouped By</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Title</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetTripChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesT = await ctx.Trips
                .Where(q => q.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new
                {
                    TripTitle = q.Title
                })
              .AsNoTracking().ToListAsync();

            var changesTG = await ctx.TripGroups
                .Include(tg => tg.Trip)
                .Where(q => q.Trip.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new
                {
                    TripTitle = q.Trip.Title,
                    GroupName = q.Name
                })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesTS = await ctx.TripSections
                .Include(ts => ts.TripGroup).ThenInclude(tg => tg.Trip)
                .Where(q => q.TripGroup.Trip.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new
                {
                    TripTitle = q.TripGroup.Trip.Title,
                    GroupName = q.TripGroup.Name,
                    SectionName = q.Name
                })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesTSS = await ctx.TripSubSections
                .Include(tss => tss.TripSection).ThenInclude(ts => ts.TripGroup).ThenInclude(tg => tg.Trip)
                .Where(q => q.TripSection.TripGroup.Trip.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new
                {
                    TripTitle = q.TripSection.TripGroup.Trip.Title,
                    GroupName = q.TripSection.TripGroup.Name,
                    SectionName = q.TripSection.Name,
                    SubSectionName = q.Name
                })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesTP = await ctx.TripPhotos
                .Include(tp => tp.Trip)
                .Include(tp => tp.TripGroup)
                .Include(tp => tp.TripSection)
                .Include(tp => tp.TripSubSection)
                .Where(q => q.Trip.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
                .Select(q => new
                {
                    TripTitle = q.Trip.Title,
                    GroupName = q.TripGroup.Name,
                    SectionName = q.TripSection.Name,
                    SubSectionName = q.TripSubSection.Name
                })
                .Distinct()
              .AsNoTracking().ToListAsync();

            var changesTC = await ctx.TripComments
                .Include(tc => tc.Trip)
                .Where(q => q.Trip.Active == true && q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new
                {
                    TripTitle = q.Trip.Title
                })
                .Distinct()
              .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesT)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.TripTitle,
                    ChangedTitle = ""
                });
            }

            foreach (var item in changesTG)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 2,
                    WhatChanged = item.TripTitle,
                    ChangedTitle = item.GroupName
                });
            }

            foreach (var item in changesTS)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 3,
                    WhatChanged = item.TripTitle,
                    ChangedTitle = item.GroupName + " - " + item.SectionName
                });
            }

            foreach (var item in changesTSS)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 4,
                    WhatChanged = item.TripTitle,
                    ChangedTitle = item.GroupName + " - " + item.SectionName + " - " + item.SubSectionName
                });
            }

            foreach (var item in changesTP)
            {
                if (item.GroupName == null)
                {
                    changesDTO.Add(new OptInChangeDTO
                    {
                        OrderBy = 5,
                        WhatChanged = item.TripTitle,
                        ChangedTitle = "Photo(s)"

                    });
                }
                else if (item.SectionName == null)
                {
                    changesDTO.Add(new OptInChangeDTO
                    {
                        OrderBy = 6,
                        WhatChanged = item.TripTitle,
                        ChangedTitle = item.GroupName + " Photo(s)"

                    });
                }
                else if (item.SubSectionName == null)
                {
                    changesDTO.Add(new OptInChangeDTO
                    {
                        OrderBy = 7,
                        WhatChanged = item.TripTitle,
                        ChangedTitle = item.GroupName + " - " + item.SectionName + " Photo(s)"

                    });
                }
                else
                {
                    changesDTO.Add(new OptInChangeDTO
                    {
                        OrderBy = 8,
                        WhatChanged = item.TripTitle,
                        ChangedTitle = item.GroupName + " - " + item.SectionName + " - " + item.SubSectionName + " Photo(s)"

                    });
                }
            }

            foreach (var item in changesTC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 9,
                    WhatChanged = item.TripTitle,
                    ChangedTitle = "Comment(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Trips</th></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetUnitChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesU = await ctx.Units
                .Where(q => q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            var changesUC = await ctx.UnitComments
                .Include(ac => ac.Unit)
                .Where(q => q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new { q.Unit.Who, q.Unit.MilitaryUnit, q.Unit.UnitLocation })
                .Distinct()
                .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesU)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.Who,
                    ChangedTitle = item.MilitaryUnit + " - " + item.UnitLocation
                });
            }

            foreach (var item in changesUC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.Who,
                    ChangedTitle = item.MilitaryUnit + " - " + item.UnitLocation + " - Comment(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Military Units</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Who</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Unit - Location</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetVideoChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesV = await ctx.Videos
                .Where(q => q.Active == true && q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            var changesVC = await ctx.VideoComments
                .Include(vc => vc.Video)
                .Where(q => q.Video.Active == true && q.CommentDate > DateTime.Today.AddDays(days))
                .Select(q => new { q.Video.GroupedBy, q.Video.Title })
                .Distinct()
              .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesV)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title
                });
            }

            foreach (var item in changesVC)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.GroupedBy,
                    ChangedTitle = item.Title + " - Comment(s)"
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Videos</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Grouped By</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Title</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetWeddings()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var users = await ctx.AppUsers
                .Where(q => q.WeddingDate != null)
              .AsNoTracking().ToListAsync();

            users.ForEach(q =>
            {
                try
                {
                    var wDate = Convert.ToDateTime(q.WeddingDate);
                    var wMon = Convert.ToDateTime(q.WeddingDate).Month;
                    var currentYr = DateTime.Now.Year;
                    if (wMon == 1)
                    {
                        currentYr++;
                    }
                    var strAnniversaryDate = wDate.Month.ToString() + "/" + wDate.Day.ToString() + "/" + DateTime.Now.Year.ToString();
                    q.AnniversaryDate = Convert.ToDateTime(strAnniversaryDate);
                }
                catch { }
            });

            users = users
                .Where(q => q.AnniversaryDate > DateTime.Today.AddDays(-1) && q.AnniversaryDate < DateTime.Today.AddDays(15))
                .ToList();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in users)
            {
                var prevItem = changesDTO.Where(q => q.ChangedTitle.Contains(item.DisplayCouple)).ToList();
                if (!prevItem.Any())
                {
                    changesDTO.Add(new OptInChangeDTO
                    {
                        OrderBy = 1,
                        WhatChanged = "Wedding Anniversaries",
                        ChangedTitle = item.DisplayCouple + " - " + Convert.ToDateTime(item.AnniversaryDate).ToString("M/d/yyyy")
                    });
                }
            }

            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;' >Wedding Anniversaries</th></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>Happy Anniversary</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }

        public async Task<string> GetWhoAmIChanges()
        {
            using var ctx = _ctxFactory.CreateDbContext();

            var changesW = await ctx.WhoAmI
                .Where(q => q.DateUpdated > DateTime.Today.AddDays(days))
              .AsNoTracking().ToListAsync();

            List<OptInChangeDTO> changesDTO = new List<OptInChangeDTO>();

            foreach (var item in changesW)
            {
                changesDTO.Add(new OptInChangeDTO
                {
                    OrderBy = 0,
                    WhatChanged = item.Owner,
                    ChangedTitle = item.Title
                });
            }

            changesDTO = changesDTO.OrderBy(a => a.WhatChanged).ThenBy(b => b.ChangedTitle).ToList();
            changesDTO = changesDTO.AsQueryable().Distinct().ToList();

            var htmlMsg = "";
            if (changesDTO.Any())
            {
                htmlMsg += "<table cellpadding=5>";
                htmlMsg += "<tr><th colspan=3 style='background-color: #DCDCDC; text-align: center;'>Who Am I</th></tr>";
                htmlMsg += "<tr><td style='background-color: #DCDCDC;'>Who</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;' style='width: 40px;'>&nbsp;</td>";
                htmlMsg += "<td style='background-color: #DCDCDC;'>Title</td></tr>";

                foreach (var item in changesDTO)
                {
                    htmlMsg += "<tr><td>" + item.WhatChanged + "</td>";
                    htmlMsg += "<td></td>";
                    htmlMsg += "<td>" + item.ChangedTitle + "</td></tr>";
                }

                htmlMsg += "</table>";
                htmlMsg += "<br />";
            }

            return htmlMsg;
        }
    }
}

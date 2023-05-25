using Microsoft.EntityFrameworkCore;
using ReviewAPI.Data;
using ReviewAPI.Interface;
using ReviewAPI.Models;

namespace ReviewAPI.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        public readonly DataContext _context;

        public ReviewerRepository(DataContext context)
        {
            _context = context;
        }



        public Reviewer GetReviewer(int reviewerId)
        {
            return _context.Reviewers.Where(o => o.Id == reviewerId).Include(p=>p.Reviews).FirstOrDefault();
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return _context.Reviewers.ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _context.Reviews.Where(p => p.Reviewer.Id == reviewerId).ToList();
        }

        public bool ReviewerExists(int reviewerId)
        {
            return _context.Reviewers.Any(i=>i.Id== reviewerId);
        }
        public bool CreateReviewer(Reviewer reviewer)
        {
            _context.Reviewers.Add(reviewer);
            return Save();
        }
        public bool Save()
        {
            return _context.SaveChanges() > 0;
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            _context.Reviewers.Update(reviewer);
            return Save();
        }
    }
}

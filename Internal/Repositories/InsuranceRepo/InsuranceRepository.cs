﻿using BHYT_BE.Internal.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Insurance = BHYT_BE.Internal.Models.Insurance;

namespace BHYT_BE.Internal.Repository.InsuranceRepo
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly InsuranceDBContext _context;
        public InsuranceRepository(InsuranceDBContext context)
        {
            _context = context;
        }
        public Insurance Create(Insurance insurance)
        {
            _context.Insurances.Add(insurance);
            _context.SaveChanges();
            return insurance;
        }

        public async Task<List<Insurance>> GetAll()
        {
            return await _context.Insurances.ToListAsync();
        }

        public async Task<Insurance> GetByID(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return await _context.Insurances.FindAsync(id);
        }

        public async Task<List<Insurance>> GetInsuranceByUserID(string userID)
        {
            return await _context.Insurances.Where(insurance => insurance.UserID == userID).ToListAsync();
        }

        public Insurance Update(Insurance insurance)
        {
            try
            {
                var result = _context.Insurances.Update(insurance);
                _context.SaveChanges();
                return result.Entity;
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movie88.Domain.Interfaces;
using Movie88.Domain.Models;
using Movie88.Infrastructure.Context;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CustomerRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CustomerModel?> GetByIdAsync(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Customerid == id);
        
        return customer != null ? _mapper.Map<CustomerModel>(customer) : null;
    }

    public async Task<CustomerModel?> GetByUserIdAsync(int userId)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Userid == userId);
        
        return customer != null ? _mapper.Map<CustomerModel>(customer) : null;
    }

    public async Task<CustomerModel?> GetByEmailAsync(string email)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.User.Email == email);
        
        return customer != null ? _mapper.Map<CustomerModel>(customer) : null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Customers.AnyAsync(c => c.User.Email == email);
    }

    public async Task<List<CustomerModel>> GetAllAsync()
    {
        var customers = await _context.Customers
            .Include(c => c.User)
            .ToListAsync();
        
        return _mapper.Map<List<CustomerModel>>(customers);
    }

    public async Task<CustomerModel> AddAsync(CustomerModel model)
    {
        var entity = _mapper.Map<Customer>(model);
        _context.Customers.Add(entity);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<CustomerModel>(entity);
    }

    public async Task<CustomerModel> UpdateAsync(CustomerModel model)
    {
        var entity = await _context.Customers.FindAsync(model.Customerid);
        if (entity == null)
            throw new KeyNotFoundException($"Customer with ID {model.Customerid} not found");

        _mapper.Map(model, entity);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<CustomerModel>(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Customers.FindAsync(id);
        if (entity == null)
            return false;

        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CustomerModel?> GetCustomerWithUserByUserIdAsync(int userId)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(c => c.Userid == userId);
        
        return customer != null ? _mapper.Map<CustomerModel>(customer) : null;
    }
}

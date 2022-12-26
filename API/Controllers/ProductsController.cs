using API.Dtos;
using API.Helpers;
using API.Helpers.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiVersion("1.0")]
[ApiVersion("1.1")]
[Authorize(Roles = "Administrator")]
//[Authorize]
public class ProductsController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    //GET: api/products
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Pager<ProductListDto>>> Get([FromQuery] Params productParams)
    {
        var result = await _unitOfWork.Products.GetAllAsync(productParams.PageIndex, productParams.PageSize, productParams.Search);

        var productsListDto = _mapper.Map<List<ProductListDto>>(result.logs);

        Response.Headers.Add("X-InlineCount", result.totalLogs.ToString());

        return new Pager<ProductListDto>(productParams.PageIndex, productParams.PageSize, result.totalLogs, productsListDto, productParams.Search);
    }

    //GET: api/products
    [HttpGet]
    [MapToApiVersion("1.1")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> NewGet()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return _mapper.Map<List<ProductDto>>(products);
    }

    //GET: api/products/4
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
            return NotFound(new ApiResponse(404, "The product doesn't exist"));

        return _mapper.Map<ProductDto>(product);
    }

    //POST: api/products
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> PostProduct(ProductAddUpdateDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        _unitOfWork.Products.Add(product);
        await _unitOfWork.SaveAsync();
        if (product == null)
        {
            return BadRequest(new ApiResponse(400));
        }
        productDto.Id = product.Id;
        return CreatedAtAction(nameof(PostProduct), new { id = productDto.Id }, productDto);
    }

    // PUT: api/products/4
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductAddUpdateDto>> Put(int id, [FromBody] ProductAddUpdateDto productDto)
    {
        if (productDto == null)
            return NotFound(new ApiResponse(404, "The product doesn't exist"));

        var productDb = await _unitOfWork.Products.GetByIdAsync(id);

        if (productDb == null)
            return NotFound(new ApiResponse(404, "The product doesn't exist"));

        var product = _mapper.Map<Product>(productDto);

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveAsync();

        return productDto;
    }

    //DELETE: api/products/4
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> Delete(int id)
    {

        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            return NotFound(new ApiResponse(404, "The product doesn't exist"));

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveAsync();

        return NoContent();
    }
}

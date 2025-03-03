using System.Collections.Immutable;
using System.Net;
using Elastic.Clients.Elasticsearch;
using ElasticSearch.API.DTOs;
using ElasticSearch.API.Models;
using ElasticSearch.API.Repositories;

namespace ElasticSearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        public ProductService(ProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            Product? responseProduct = await _productRepository.SaveAsync(request.CreateProduct());

            if (responseProduct == null)
                return ResponseDto<ProductDto>.Fail(new List<string> { "Kayıt edilemedi!" }, HttpStatusCode.InternalServerError);

            return ResponseDto<ProductDto>.Success(responseProduct.CreateDto(), HttpStatusCode.Created);
        }

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            ImmutableList<Product> products = await _productRepository.GetAllAsync();

            List<ProductDto> productListDto = products.Select(x => new ProductDto(
                x.Id,
                x.Name,
                x.Price,
                x.Stock,
                x.Feature != null ? new ProductFeatureDto(x.Feature.Width, x.Feature.Height, x.Feature.Color.ToString()) : null
            )).ToList();

            return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
        {
            var hasProduct = await _productRepository.GetByIdAsync(id);

            if(hasProduct == null)
                return ResponseDto<ProductDto>.Fail("Ürün bulunamadı!", HttpStatusCode.NotFound);

            return ResponseDto<ProductDto>.Success(hasProduct.CreateDto(), HttpStatusCode.OK);
        }

        public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto updateProduct)
        {
            bool isSuccess = await _productRepository.UpdateAsync(updateProduct);
            if(!isSuccess)
                return ResponseDto<bool>.Fail(new List<string> { "Güncelleme yapılamadı!" }, HttpStatusCode.InternalServerError);

            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }

        public async Task<ResponseDto<bool>> DeleteAsync(string id)
        {
            var deleteResponse = await _productRepository.DeleteAsync(id);

            //Nest Kütüphanesi
            //if(!deleteResponse.IsValid && deleteResponse.Result == Result.NotFound)

            //Elasticsearch Kütüphanesi
            if (!deleteResponse.IsValidResponse && deleteResponse.Result == Result.NotFound)
                return ResponseDto<bool>.Fail(new List<string> { "Silmeye çalıştığınız ürün bulunamadı!" }, HttpStatusCode.NotFound);

            if (!deleteResponse.IsValidResponse)
            {
                //Nest Kütüphanesi
                //_logger.LogError(deleteResponse.OriginalException, deleteResponse.ServerError.Error.ToString());

                //Elasticsearch Kütüphanesi
                deleteResponse.TryGetOriginalException(out Exception? exception);
                _logger.LogError(exception, deleteResponse.ElasticsearchServerError!.Error.ToString());
                return ResponseDto<bool>.Fail(new List<string> { "Silme işleminde bir hata meydana geldi!" }, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }
    }   
}

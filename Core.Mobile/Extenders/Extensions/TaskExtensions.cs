//using Microsoft.AppCenter.Crashes;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Core
{
    internal static class TaskExtensions
    {
        public static async Task<ApiResult<bool>> Handle(this Task self, BaseViewModel vm = null) {
            SetBusy(vm, true);

            try {
                await self.ConfigureAwait(false);
                return ApiResult.Create(true, true, HttpStatusCode.OK);
            } catch (ApiException ex) {
                return Error<bool>(ex);
            } catch (Exception ex) {
                return Error<bool>(ex);
            } finally {
                SetBusy(vm, false);
            }
        }

        public static async Task<ApiResult<T>> Handle<T>(this Task<T> self, BaseViewModel vm = null) {
            SetBusy(vm, true);

            try {
                var result = await self.ConfigureAwait(false);
                return ApiResult.Create(result, true, HttpStatusCode.OK);
            } catch (ApiException apiEx) {
                return Error<T>(apiEx);
            } catch (System.Exception ex) {
                return Error<T>(ex);
            } finally {
                SetBusy(vm, false);
            }
        }

        static void SetBusy(BaseViewModel vm, bool value) {
            if (vm != null) vm.IsBusy = value;
        }

        static ApiResult<T> Error<T>(ApiException apiEx) {
            //Crashes.TrackError(apiEx);

            switch (apiEx.StatusCode) {
                case HttpStatusCode.BadRequest:
                    return ApiResult.Create(default(T), false, apiEx.StatusCode, apiEx.Message);
                default:
                    return ApiResult.Create(default(T), false, apiEx.StatusCode, "Não foi possível executar a ação no servidor");
            }
        }

        static ApiResult<T> Error<T>(Exception ex) {
           // Crashes.TrackError(ex);
            return ApiResult.Create(default(T), false, HttpStatusCode.ExpectationFailed, "Não foi possível executar a ação no servidor");
        }
    }
}

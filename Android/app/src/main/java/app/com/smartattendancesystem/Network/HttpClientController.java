package app.com.smartattendancesystem.Network;

import java.util.ArrayList;
import java.util.List;

import javax.net.ssl.HostnameVerifier;
import javax.net.ssl.SSLSession;

import app.com.smartattendancesystem.BuildConfig;
import okhttp3.Interceptor;
import okhttp3.OkHttpClient;
import okhttp3.logging.HttpLoggingInterceptor;

public class HttpClientController
{
    private OkHttpClient okHttpClient;
    private OkHttpClient picassoOkHttpClient;

    private static HttpClientController instance;

    public static HttpClientController getInstance()
    {
        if (instance == null)
        {
            instance = new HttpClientController();
        }
        return instance;
    }

    private HttpClientController()
    {
        OkHttpClient.Builder builder = new OkHttpClient.Builder();

        //builder.interceptors().addAll(getInterceptorList());
        if (BuildConfig.DEBUG) {
            HttpLoggingInterceptor logging = new HttpLoggingInterceptor();
            logging.setLevel(HttpLoggingInterceptor.Level.BODY);

            builder.addInterceptor(logging);
        }

        /*builder = builder.connectTimeout(30, TimeUnit.SECONDS);
        builder = builder.writeTimeout(30, TimeUnit.SECONDS);
        builder = builder.readTimeout(30, TimeUnit.SECONDS);*/

        try
        {
            builder = builder.hostnameVerifier(new HostnameVerifier() {
                @Override
                public boolean verify(String s, SSLSession sslSession)
                {
                    return true;
                }
            });
            /*TLSSocketFactory tlsSocketFactory = new TLSSocketFactory();
            builder = builder.sslSocketFactory(tlsSocketFactory, tlsSocketFactory.systemDefaultTrustManager());*/
        }
        catch (Exception ex)
        {
            ex.printStackTrace();
        }

        okHttpClient = builder.build();

        /*OkHttpClient.Builder picassoOkhttpBuilder = new OkHttpClient.Builder();
        picassoOkhttpBuilder.interceptors().addAll(getPicassoInterceptorList());
        picassoOkhttpBuilder = picassoOkhttpBuilder.connectTimeout(30, TimeUnit.SECONDS);
        picassoOkhttpBuilder = picassoOkhttpBuilder.writeTimeout(60, TimeUnit.SECONDS);
        picassoOkhttpBuilder = picassoOkhttpBuilder.readTimeout(30, TimeUnit.SECONDS);
        picassoOkHttpClient = picassoOkhttpBuilder.build();*/
    }

    public List<Interceptor> getInterceptorList()
    {
        List<Interceptor> interceptorList = new ArrayList<>();
        //interceptorList.add(new RequestInterceptor());
        HttpLoggingInterceptor loggingInterceptor = new HttpLoggingInterceptor();
        loggingInterceptor.setLevel(HttpLoggingInterceptor.Level.BODY);
        interceptorList.add(loggingInterceptor);
        return interceptorList;
    }

    /*public List<Interceptor> getPicassoInterceptorList()
    {
        List<Interceptor> interceptorList = new ArrayList<>();
        interceptorList.add(new PicassoRequestInterceptor());
        interceptorList.add(new RequestInterceptor());
        HttpLoggingInterceptor loggingInterceptor = new HttpLoggingInterceptor();
        loggingInterceptor.setLevel(HttpLoggingInterceptor.Level.BODY);
        interceptorList.add(loggingInterceptor);
        return interceptorList;
    }*/

    /*private X509TrustManager getTrustManager()
    {
        return new X509TrustManager()
        {
            @Override
            public void checkClientTrusted(X509Certificate[] chain, String authType) throws CertificateException
            {

            }

            @Override
            public void checkServerTrusted(X509Certificate[] chain, String authType) throws CertificateException
            {

            }

            @Override
            public X509Certificate[] getAcceptedIssuers()
            {
                return new X509Certificate[0];
            }
        };
    }*/

    public OkHttpClient getHttpClient()
    {
        return okHttpClient;
    }

   /* public OkHttpClient getPicassoOkHttpClient()
    {
        return picassoOkHttpClient;
    }*/
}

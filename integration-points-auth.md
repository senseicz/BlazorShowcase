# User Authentication, Integration with OnePass, Duende BFF

## Server side

* .csproj file

        <PackageReference Include="Duende.BFF" Version="1.1.2" />

* Startup.cs

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddBff();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookie";
                    options.DefaultChallengeScheme = "oidc";
                    options.DefaultSignOutScheme = "oidc";
                })
                .AddCookie("cookie", options =>
                {
                    options.Cookie.Name = "__Host-blazor";
                    options.Cookie.SameSite = SameSiteMode.Strict;
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://authcolleague-absaaccess-uat.intra.absaafrica";

                    // confidential client using code flow + PKCE
                    options.ClientId = "blazor-showcase-tonda";
                    options.ClientSecret = "*****";
                    options.ResponseType = "code";
                    options.ResponseMode = "query";

                    options.MapInboundClaims = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    // request scopes + refresh tokens
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("offline_access");
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseBff();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBffManagementEndpoints();

                endpoints.MapControllers()
                    .RequireAuthorization()
                    .AsBffApiEndpoint();
            });
        }

## Client side

* .csproj file

        <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" Version="6.0.1" />

* Program.cs

        // authentication state and authorization
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();

        // Supply HttpClient instances that include access tokens when making requests to the server project
        builder.Services.AddTransient<AntiforgeryHandler>();

        builder.Services.AddHttpClient("backend", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)).AddHttpMessageHandler<AntiforgeryHandler>();
        
        builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("backend"));

* AntiforgeryHandler.cs
* BffAuthenticationStateProvider.cs







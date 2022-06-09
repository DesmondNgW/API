function Provider(Client) {
    this.Client = Client;
}

function ClientProvider(Client, ReleaseClient, Dispose) {
    Provider.call(this, Client);
    this.ReleaseClient = ReleaseClient;
    this.Dispose = Dispose;
}

ClientProvider.prototype = new Provider();
ClientProvider.prototype.constructor = ClientProvider;

export const IProvider = {
    Provider: Provider,
    ClientProvider: ClientProvider
};
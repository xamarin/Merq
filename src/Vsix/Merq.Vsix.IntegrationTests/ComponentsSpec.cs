using System;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Xunit;
using Xunit.Abstractions;

namespace Merq
{
	public class ComponentsSpec
	{
		ITestOutputHelper output;

		public ComponentsSpec(ITestOutputHelper output)
		{
			this.output = output;
		}

		[VsixFact(RunOnUIThread = true)]
		public void when_subscribing_and_pushing_events_then_succeeds()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var stream = ServiceProvider.GlobalProvider.GetService<SComponentModel, IComponentModel>().GetService<IMessageBus>();
			var expected = new FooEvent();

			FooEvent actual = null;

			var subscription = stream.Observe<FooEvent>().Subscribe(e => actual = e);

			stream.Notify(expected);

			Assert.Same(expected, actual);
		}

		[VsixFact(RunOnUIThread = true)]
		public void when_querying_command_bus_for_handler_then_succeeds()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var commands = ServiceProvider.GlobalProvider.GetService<SComponentModel, IComponentModel>().GetService<IMessageBus>();

			Assert.False(commands.CanHandle(new FooCommand()));
			Assert.False(commands.CanHandle<FooAsyncCommand>());
		}

		public class FooCommand : ICommand { }

		public class FooAsyncCommand : IAsyncCommand { }

		public class FooEvent { }
	}
}
